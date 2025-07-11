#!/usr/bin/env bash
set -e

INPUT_HTML="$1"
OUTPUT_HTML="${2:-$INPUT_HTML}"

cp "$INPUT_HTML" "$OUTPUT_HTML"

# Inline CSS
grep -oP '<link[^>]+rel="stylesheet"[^>]+href="[^"]+"' "$INPUT_HTML" | \
while read -r line; do
  href=$(echo "$line" | grep -oP 'href="\K[^"]+')
  if [[ -f "$href" ]]; then
    style="<style>$(cat "$href")</style>"
    sed -i "s|$line[^>]*>|$style|" "$OUTPUT_HTML"
  fi
done

# Inline JS
grep -oP '<script[^>]+src="[^"]+"' "$INPUT_HTML" | \
while read -r line; do
  src=$(echo "$line" | grep -oP 'src="\K[^"]+')
  if [[ -f "$src" ]]; then
    script="<script>$(cat "$src")</script>"
    sed -i "s|$line[^>]*></script>|$script|" "$OUTPUT_HTML"
  fi
done
