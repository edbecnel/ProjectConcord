#!/usr/bin/env bash
set -euo pipefail

source_png="$1"
output_icns="$2"
work_dir="$(mktemp -d)"
iconset_dir="$work_dir/ProjectConcord.iconset"
square_png="$work_dir/square-source.png"

cleanup() {
  rm -rf "$work_dir"
}
trap cleanup EXIT

mkdir -p "$iconset_dir"

# Pad to a square canvas (no stretch) so non-square masters survive iconutil.
width="$(sips -g pixelWidth "$source_png" | awk '/pixelWidth:/ {print $2}')"
height="$(sips -g pixelHeight "$source_png" | awk '/pixelHeight:/ {print $2}')"
max_dim=$(( width > height ? width : height ))
sips --padToHeightWidth "$max_dim" "$max_dim" "$source_png" --out "$square_png" >/dev/null

for size in 16 32 128 256 512; do
  sips -z "$size" "$size" "$square_png" --out "$iconset_dir/icon_${size}x${size}.png" >/dev/null
  double=$((size * 2))
  sips -z "$double" "$double" "$square_png" --out "$iconset_dir/icon_${size}x${size}@2x.png" >/dev/null
done

iconutil -c icns "$iconset_dir" -o "$output_icns"
