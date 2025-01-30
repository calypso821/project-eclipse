import os
import sys
import re

def clean_duplicates(folder):
    # Walk through all directories recursively
    for root, _, files in os.walk(folder):
        # First find all files with (1) in their name
        for filename in files:
            match = re.match(r'^(.+)\(1\)(\.[^.]+)$', filename)
            if match:
                base_name = match.group(1)
                extension = match.group(2)
                original_file = base_name + extension
                
                # Full paths
                original_path = os.path.join(root, original_file)
                duplicate_path = os.path.join(root, filename)
                
                # If original exists, delete it and rename the (1) version
                if os.path.exists(original_path):
                    try:
                        os.remove(original_path)
                        os.rename(duplicate_path, original_path)
                        print(f"Cleaned: {original_path}")
                    except Exception as e:
                        print(f"Error processing {duplicate_path}: {e}")

def main():
    if len(sys.argv) != 2:
        print("Usage: python clean.py <folder>")
        return
        
    folder = sys.argv[1]
    if not os.path.exists(folder):
        print(f"Error: Folder '{folder}' does not exist")
        return
        
    clean_duplicates(folder)
    print("Cleanup complete")

if __name__ == "__main__":
    main()