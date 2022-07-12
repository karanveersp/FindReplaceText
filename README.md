# FindReplaceText

This app allows replacing text patterns in files matched with a glob pattern
under a specified directory.

```
USAGE: FindReplaceText [--help] [--directory-path <path>] [--file-pattern <pattern>] [--text-pattern <pattern>] [--replacement <string>] [--dryrun]

OPTIONS:

    --directory-path <path>
                          specify directory containing files to match recursively
    --file-pattern <pattern>
                          specify a glob pattern to match files
    --text-pattern <pattern>
                          specify a regex pattern to match text in matched files
    --replacement <string>
                          replacement text
    --dryrun              perform a dry run (print updates without making them)
    --help                display this list of options.
```
