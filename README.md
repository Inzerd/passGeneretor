# passGeneretor

passGenerator tool allow user to create a custom worldlist to generate password or other list of string.
passGenerator (at this point the tool) have two execution modality that can work togher:
        
        - permutation modality: gived a list of string, return for any term all swap combination defined in
        passComposerConfiguration.json or passed via command line
        for example: gived "mouse" -> in configuration we set char "e" to swap in ["3","&"] and the result are:
        ["m0use","m0us3","m0us&","mous3","mous&"]
        
        - combination modality: gived a list of string, return a new list with all possibile combination, for example: gived ["a","b","c"] -> return ["abc","acb","bac","bca","cba","cab"], if in  configuration there are "DelimitatorList" valorized, "tool" use this list to create new combination other those above showed.
        The number of term used for combination are defined in configuration file

this two modality can works togher, first start permutation modality to generate the lists to combination, and after start combination, the combination modality not combination term from same "radice", this means that tool need almost two term passed to create combination,
for example run: 'passComposer -p -c -u ./listOfTerm.txt" -o /output.txt -l 8'

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

Summary:
        -c combination: combined any trasfomartion will creating more pass complex
        -p permutation: substitute char in term with Trasformation List rule in configuration json file
        -h manual: show manual in console

FUTURE RELEASED:

Permutation options:

syllabe: take the string term passed from user and return new list with same term but divide from syllabe (many composition)

camelCase: given a single term return its camelCase rapprents

lowerUpper: gived a single term return a list of term with all combination of upper and lower case.



