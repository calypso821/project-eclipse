from PIL import Image
import os
import sys

def trim_image(image_path):
    img = Image.open(image_path).convert('RGBA')
    bbox = img.getbbox()
    
    if not bbox:
        print(f"Skipping: {image_path} (completely transparent)")
        return None
        
    if bbox == (0, 0, img.width, img.height):
        print(f"Skipping: {image_path} (already minimally cropped)")
        return None
        
    return img.crop(bbox)

def process_directory(input_dir, output_dir):
    # Create corresponding output directory
    os.makedirs(output_dir, exist_ok=True)
    
    # Process all files and subdirectories
    for item in os.listdir(input_dir):
        input_path = os.path.join(input_dir, item)
        output_path = os.path.join(output_dir, item)
        
        # If it's a directory, process it recursively
        if os.path.isdir(input_path):
            process_directory(input_path, output_path)
        # If it's a PNG file, process it
        elif item.lower().endswith('.png'):
            try:
                trimmed = trim_image(input_path)
                if trimmed:
                    # If input and output dirs are same, append (1)
                    if input_dir == output_dir:
                        base, ext = os.path.splitext(output_path)
                        output_path = f"{base}(1){ext}"
                    trimmed.save(output_path)
                    print(f"Saved: {output_path}")
                    print(f"Dimensions: {trimmed.size}")
            except Exception as e:
                print(f"Error processing {input_path}: {e}")

def main():
    if len(sys.argv) not in [2, 3]:
        print("Usage: python trim.py <input_folder> [output_folder]")
        return
        
    input_folder = sys.argv[1]
    output_folder = sys.argv[2] if len(sys.argv) == 3 else input_folder
    
    process_directory(input_folder, output_folder)

if __name__ == "__main__":
    main()