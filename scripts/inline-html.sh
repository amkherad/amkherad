#!/usr/bin/env bash
set -e

INPUT_HTML="$1"
OUTPUT_HTML="${2:-$INPUT_HTML}"
TEMP_HTML="$(mktemp)"

cp "$INPUT_HTML" "$TEMP_HTML"

# Inline CSS
while read -r line; do
  if [[ "$line" =~ \<link.*rel=\"stylesheet\".*href=\"([^\"]+)\".*\> ]]; then
    css_path="${BASH_REMATCH[1]}"
    if [[ -f "$css_path" ]]; then
      echo "Inlining CSS: $css_path"
      echo "<style>" >> "$TEMP_HTML.tmp"
      cat "$css_path" >> "$TEMP_HTML.tmp"
      echo "</style>" >> "$TEMP_HTML.tmp"
    else
      echo "⚠️ CSS file not found: $css_path"
      echo "$line" >> "$TEMP_HTML.tmp"
    fi
  else
    echo "$line" >> "$TEMP_HTML.tmp"
  fi
done < "$TEMP_HTML"

mv "$TEMP_HTML.tmp" "$TEMP_HTML"

# Inline JS
while read -r line; do
  if [[ "$line" =~ \<script.*src=\"([^\"]+)\".*\>\<\/script\> ]]; then
    js_path="${BASH_REMATCH[1]}"
    if [[ -f "$js_path" ]]; then
      echo "Inlining JS: $js_path"
      echo "<script>" >> "$TEMP_HTML.tmp"
      cat "$js_path" >> "$TEMP_HTML.tmp"
      echo "</script>" >> "$TEMP_HTML.tmp"
    else
      echo "⚠️ JS file not found: $js_path"
      echo "$line" >> "$TEMP_HTML.tmp"
    fi
  else
    echo "$line" >> "$TEMP_HTML.tmp"
  fi
done < "$TEMP_HTML"

mv "$TEMP_HTML.tmp" "$OUTPUT_HTML"
echo "✅ Done. Output written to $OUTPUT_HTML"
