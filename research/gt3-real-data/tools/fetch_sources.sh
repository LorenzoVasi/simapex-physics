#!/usr/bin/env bash
# Re-download every source listed in sources_index.csv into sources/ (git-ignored)
# and write sources_fetch_<date>.tsv so hashes can be compared with the index.
# Run from research/gt3-real-data:  bash tools/fetch_sources.sh
set -euo pipefail
here="$(cd "$(dirname "$0")/.." && pwd)"
mkdir -p "$here/sources"
out="$here/sources_fetch_$(date +%F).tsv"
tail -n +2 "$here/sources_index.csv" | awk -F',' '{id=$1; sub(/^SRC-0*/,"",id); u=$2; gsub(/"/,"",u); print id "\t" u}' \
  | xargs -P 8 -d '\n' -I{} bash -c 'i=$(cut -f1 <<<"{}"); u=$(cut -f2 <<<"{}"); bash "'"$here"'/tools/fetch_one.sh" "$i" "$u" "'"$here"'/sources"' \
  | sort -n > "$out"
echo "Wrote $out. Compare column 6 (sha256) with sources_index.csv."
