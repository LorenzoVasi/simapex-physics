#!/usr/bin/env bash
# Usage: fetch_one.sh <index> <url> <outdir>
# Downloads one URL as raw bytes (never executed) and prints a TSV line:
# index, url, http_code, content_type, size_bytes, sha256, file, final_url
i="$1"; url="$2"; out="$3"
host=$(echo "$url" | awk -F/ '{print $3}' | tr -c 'A-Za-z0-9.-' '_')
base=$(printf '%03d_%s' "$i" "$host")
tmp="$out/$base.part"
meta=$(curl -sS -L --max-time 120 --max-filesize 314572800 --compressed \
  -A "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0 Safari/537.36" \
  -H "Accept-Language: en,it;q=0.8" \
  -o "$tmp" -w '%{http_code}\t%{content_type}\t%{url_effective}' "$url" 2>/dev/null)
rc=$?
code=$(echo "$meta" | cut -f1); ctype=$(echo "$meta" | cut -f2); final=$(echo "$meta" | cut -f3)
[ -z "$code" ] && code="ERR$rc"
ext="bin"
case "$ctype" in
  *pdf*) ext="pdf";; *html*) ext="html";; *json*) ext="json";; *zip*) ext="zip";;
  *csv*) ext="csv";; *xml*) ext="xml";; *text/plain*) ext="txt";; *image/png*) ext="png";; *image/jpeg*) ext="jpg";;
esac
if [ -s "$tmp" ]; then
  f="$base.$ext"; mv -f "$tmp" "$out/$f"
  size=$(stat -c %s "$out/$f"); sha=$(sha256sum "$out/$f" | cut -d' ' -f1)
else
  rm -f "$tmp"; f=""; size=0; sha=""
fi
printf '%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\n' "$i" "$url" "$code" "$ctype" "$size" "$sha" "$f" "$final"
