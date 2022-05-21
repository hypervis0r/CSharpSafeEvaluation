# CSharpSafeEvaluation
A small experiment to make runtime C# script evaluation safer.

## Things that are allowed
* Arithmetic (`+` `-` `*` `/`)
* Comparisons (`<` `>` `<=` `>=` `==` `!=`)
* If statements (`if (1 == 1) { /* do thing */ }`)
* Literals (`1234` `"hello"`)
* Bitwise (`~` `<<` `>>` `&` `|`)
* Logical operators (`&&` `||`)
* Parentheses `(1 == 2)`
* Calling only specified built in functions

Currently the scripts only return `bool`, but can be easily modified to return other things.
