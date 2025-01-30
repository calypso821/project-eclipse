from PIL import Image
import os
import sys

def generate_unique_filename(output_path):
    base, ext = os.path.splitext(output_path)
    counter = 1
    while os.path.exists(output_path):
        output_path = f"{base}({counter}){ext}"
        counter += 1
    return output_path

def resize_image(image_path, output_path, scale=None, height=None, width=None):
    img = Image.open(image_path)
    orig_width, orig_height = img.size

    if scale:
        if scale >= 1:
            print(f"Skipping {image_path}: Scale must be less than 1 to prevent upscaling")
            return
        new_width = int(orig_width * scale)
        new_height = int(orig_height * scale)
    elif height:
        if height >= orig_height:
            print(f"Skipping {image_path}: Target height ({height}) must be less than image height")
            return
        ratio = height / orig_height
        new_width = int(orig_width * ratio)
        new_height = height
    elif width:
        if width >= orig_width:
            print(f"Skipping {image_path}: Target width ({width}) must be less than image width")
            return
        ratio = width / orig_width
        new_width = width
        new_height = int(orig_height * ratio)
    else:
        raise ValueError("Specify either scale (s), height (h), or width (w)")

    if (new_width, new_height) == (orig_width, orig_height):
        print(f"Skipping {image_path}: Already at target size")
        return

    resized = img.resize((new_width, new_height), Image.Resampling.LANCZOS)
    resized.save(output_path)
    print(f"Saved: {output_path}")
    print(f"New size: {new_width}x{new_height}")

def process_directory(input_dir, output_dir, scale=None, height=None, width=None):
    os.makedirs(output_dir, exist_ok=True)
    
    for item in os.listdir(input_dir):
        input_path = os.path.join(input_dir, item)
        output_path = os.path.join(output_dir, item)
        
        if os.path.isdir(input_path):
            process_directory(input_path, output_path, scale, height, width)
        elif item.lower().endswith('.png'):
            if input_dir == output_dir:
                output_path = generate_unique_filename(output_path)
            try:
                resize_image(input_path, output_path, scale, height, width)
            except Exception as e:
                print(f"Error processing {input_path}: {e}")

def main():
    if len(sys.argv) < 3:
        print("Usage: python resize.py <resize_flags> <input_folder> [output_folder]")
        print("Flags: s=0.5 or h=300 or w=500")
        print("Note: Only downscaling is allowed (scale must be less than 1)")
        return

    flag = sys.argv[1]
    scale = height = width = None
    if flag.startswith("s="): 
        scale = float(flag[2:])
        if scale <= 0 or scale >= 1:
            print("Error: Scale must be between 0 and 1")
            return
    elif flag.startswith("h="): 
        height = int(flag[2:])
        if height <= 0:
            print("Error: Height must be greater than 0")
            return
    elif flag.startswith("w="): 
        width = int(flag[2:])
        if width <= 0:
            print("Error: Width must be greater than 0")
            return

    input_folder = sys.argv[2]
    output_folder = sys.argv[3] if len(sys.argv) == 4 else input_folder
    
    process_directory(input_folder, output_folder, scale, height, width)

if __name__ == "__main__":
    main()