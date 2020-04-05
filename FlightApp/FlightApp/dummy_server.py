import argparse
import socket
import logging
import math
from typing import Dict, List, Tuple

DEFAULT_LISTEN_PORT = 5402 
THROTTLE = '/controls/engines/current-engine/throttle'
RUDDER = '/controls/flight/rudder'
LATITUDE = '/position/latitude-deg'
EXAMPLE_FIELDS = {THROTTLE: (-1, 1), 
                  RUDDER: (-1, 1), 
                  LATITUDE: (-90, 90)}

class FlightSimulator:

    def __init__(self, variable_ranges: Dict):
        self.variable_ranges = variable_ranges
        self.variables = {var_name: (var_range[0] + var_range[1]) / 2
                          for var_name, var_range in variable_ranges.items()}
        self.commands = {'get': self.get_value, 'set': self.set_value}

    def set_value(self, var_name: str, var_value: float):
        min_range, max_range = self.variable_ranges[var_name]
        if var_value < min_range:
            var_value = min_range
        if var_value > max_range:
            var_value = max_range
        self.variables[var_name] = var_value
        return self.get_value(var_name)

    def get_value(self, var_name: str):
        var_value = self.variables[var_name]
        return str(var_value)

    def process(self, command: str):
        try:
            command_type, args = self.parse(command)
            func = self.commands[command_type]
            result = func(*args)
            return result
        except ValueError:
            return "ERR"

    def __call__(self, command: str):
        return self.process(command)

    def parse(self, command: str):
        tokens = command.split()
        if len(tokens) < 2:
            raise ValueError(f"Command must be have at least two tokens,"
                             f" given: {command}")

        if tokens[0] not in self.commands:
            raise ValueError(f'Command must start with get or set,'
                             f' given: {command}')

        if tokens[1] not in self.variables:
            raise ValueError("Variable name not found, given: {command}")

        if tokens[0] == 'set' and len(tokens) != 3:
            raise ValueError('Set command must have variable and value,'
                             f'given: Invalid command {command}')

        command_type = tokens[0]
        var_name = tokens[1]
        if command_type == "set":
            var_value = float(tokens[2])
            return command_type, (var_name, var_value)
        else:
            return command_type, (var_name,)


class Server:
    def __init__(self, port, chunk_size=1024):
        self.port = port
        self.chunk_size = chunk_size

    def serve(self, message_handler):
        with socket.socket(family=socket.AF_INET, type=socket.SOCK_STREAM) as the_socket:
            logger.info(f'Binding to port: {self.port} on all available devices')
            the_socket.bind(('', self.port))
            logger.info(f"Listening on port {self.port}")
            the_socket.listen()
            client_socket, client_address = the_socket.accept()
            logger.info(f"Accepted a connection from: {client_address}")
            self.serve_client(client_socket, message_handler)

    def serve_client(self, client_socket: socket.socket, message_handler):
        buffer = ''
        with client_socket:
            data = client_socket.recv(self.chunk_size)
            while data:
                text = data.decode('ascii')
                # Concatenate whatever comes from the client
                # with the previous unhandled partial message
                buffer += text
                commands, buffer = Server.process_text(buffer)
                for command in commands:
                    result = message_handler(command) + '\n'
                    result_data = result.encode('ascii')
                    client_socket.sendall(result_data)
                data = client_socket.recv(self.chunk_size)

    @staticmethod
    def process_text(buffer):
        # Keep ends for the case where the last line
        # doesn't end with a line boundary (\n) and must
        # be concatenated with the next input from the client
        lines = buffer.splitlines(keepends=True)
        is_all_complete = lines[-1].splitlines()[0] != lines[-1]
        next_buffer = ''
        if not is_all_complete:
            next_buffer = lines[-1]
            lines = lines[:-1]
        commands = [line.strip() for line in lines if line.strip()]
        # last unprocessed text (didn't end with a newline)
        # becomes the new buffer
        return commands, next_buffer


def main(args):
    server = Server(args.listen_port)
    simulator = FlightSimulator(EXAMPLE_FIELDS)
    server.serve(simulator)


if __name__ == '__main__':
    logger = logging.getLogger("FlightSimulator")
    logger.setLevel(logging.DEBUG)
    parser = argparse.ArgumentParser()
    parser.add_argument('--listen-port', default=DEFAULT_LISTEN_PORT, type=int)
    main(parser.parse_args())
