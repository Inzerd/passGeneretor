# passGeneretor

passGeneretor is a tool for generating customized passwords and string lists. It supports two main modes: permutation and combination, which can be used together to create complex passwords.

## Installation

To build the project, make sure you have the .NET SDK installed. You can build the project by running:

```sh
dotnet build --configuration Release
```

## Usage

The main command to run the tool is `passComposer`. Below are the supported parameters:

### Required Parameters

- `-u [url]`: Specifies the user input file.
- `-l [length]`: Specifies the length of the password.

### Options

- `-rule [cns]`: Rule for generating the password. This option generates a regexp used to validate the generated password:
  - `c`: Use uppercase and lowercase letters.
  - `n`: Use numbers.
  - `s`: Use special characters.
- `-o [url]`: Specifies the output file. If not specified, the tool saves the output in "passGeneraThor_output.txt".
- `-t [url]`: Specifies the file with transformations from character/string to number/special character/string or another transformation string.
- `-c`: Enables combination mode, which combines all transformations to create more complex passwords.
- `-p`: Enables permutation mode, which replaces characters in the terms with the rules from the transformation list in the JSON configuration file.
- `-h`: Shows the manual in the console.

### Example Usage

```sh
passgeneretor -p -c -u ./listOfTerm.txt -o /output.txt -l 8
```
Returns a list of passwords with a length of 8 characters, generated from a combination and permutation of terms in `listOfTerm.txt`.

```sh
passgeneretor -p -u ./listOfTerm.txt -l 8
```
Returns a list of passwords with a length of 8 characters, generated from the permutation of terms in `listOfTerm.txt`.

```sh
passgeneretor -c -u ./listOfTerm.txt -l 8
```
Returns a list of passwords with a length of 8 characters, generated from the combination of terms in `listOfTerm.txt`.

```sh
passgeneretor -u ./listOfTerm.txt -l 8 -rule cn
```
Returns a list of terms filtered by length and password rules (uppercase, lowercase, and numbers).

## Configuration

The configuration file `passComposerConfiguration.json` should be included in the config directory and contains the rules for character transformations.

## License

This project is licensed under the MIT License. See the LICENSE file for more details. passGeneretor