# passGeneretor

Mandatory:
        -u [url]: file info user mandatory
        -o [url]: output file
        -l [lenght]: password's lenght
        -c/-p: at least one of these options

Options:
        -rule [cns]: rule from generate password, this options generate a regexp used for validate genereted password:
                c = use Major and Minor Case
                n = use number
                s = use special char

        -t [url]: file with transformation from char/ string to number/ special char/ string or another transformation string
        -c combination: combined any trasfomartion will creating more pass complex
        -p permutation: substitute char in term with Trasformation List rule in configuration json file
        -h manual: show manual in console


permutataion: Given a list of terms replaces each character qith the past configuration.
Proceed to create any possible permutation combination 

combination: Given a Dictionary of terms genereted previously by the permutation proceeds to combine each term for any key, returning every possbile combination.
Each combination before being written to output files is checked by the rule passed to the tool.        

FUTURE RELEASED:

Permutation options:

syllabe: take the string term passed from user and return new list with same term but divide from syllabe (many composition)

camelCase: given a single term return its camelCase rapprents

lowerUpper: gived a single term return a list of term with all combination of upper and lower case.



