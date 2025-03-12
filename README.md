# SPLIT MARKDOWN

Takes one large Markdown file and splits it based on a chosen heading level.

A `_split` folder is created with one file for each resulting section, named as per the heading.
Confirmation is requested before an existing destination folder is replaced.

[MIT License](./LICENSE)

## Usage

``` shell
SplitMarkdown <source> <heading> <numbered?>
```

| Argument | Details                                   | Examples   |
|:---------|:------------------------------------------|:-----------|
| source   | The Markdown file holding all the content | `novel.md` |
| heading  | Which heading to split at                 | `2`        |
| numbered | Number the output files in sequence?      | `true`     |

The `novel.md` file may contain something like this:

``` markdown
---
Title:  My Book
---

## Chapter One

Lorem ipsum dolor sit amet adipiscing consectetur elit.

## Chapter Two

Lorem ipsum dolor sit amet adipiscing consectetur elit.
```

This would create something like this:

```
/_split
    _list.yaml
    _untitled.md
    001 Chapter One.md
    002 Chapter Two.md
novel.md
```

- `_untitled.md` holds any content found before the first matching header.
Often this will be leading YAML from the original Markdown.
- `_list.yaml` contains a `Contents:` list of the individual files in the sequence they were discovered
