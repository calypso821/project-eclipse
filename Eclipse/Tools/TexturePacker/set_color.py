from PIL import Image
import os
import argparse

def convert_to_color_transparent(input_path, output_path, color=(255, 255, 255)):
    """
    Convert image to specified color while preserving transparency.
    """
    img = Image.open(input_path)
    
    if img.mode != 'RGBA':
        img = img.convert('RGBA')
    
    alpha = img.split()[3]
    colored_img = Image.new('RGBA', img.size, (*color, 255))
    colored_img.putalpha(alpha)
    colored_img.save(output_path)

def process_directory(input_dir, output_dir=None, color=(255, 255, 255)):
    """
    Process all images in a directory and its subdirectories recursively.
    """
    if not os.path.isdir(input_dir):
        raise ValueError(f"Input path '{input_dir}' is not a directory")
    
    # If no output directory specified, use input directory
    if output_dir is None:
        output_dir = input_dir
    
    # Supported image formats
    supported_formats = ['.png', '.jpg', '.jpeg', '.gif', '.bmp']
    
    # Walk through directory recursively
    for root, dirs, files in os.walk(input_dir):
        # Calculate relative path from input directory
        rel_path = os.path.relpath(root, input_dir)
        
        # Create corresponding output directory
        current_output_dir = os.path.join(output_dir, rel_path) if rel_path != '.' else output_dir
        os.makedirs(current_output_dir, exist_ok=True)
        
        for filename in files:
            if any(filename.lower().endswith(fmt) for fmt in supported_formats):
                input_path = os.path.join(root, filename)
                
                # If same directory, add (1) to filename
                if output_dir == input_dir:
                    name, ext = os.path.splitext(filename)
                    output_filename = f"{name}(1){ext}"
                else:
                    output_filename = filename
                    
                output_path = os.path.join(current_output_dir, output_filename)
                
                try:
                    convert_to_color_transparent(input_path, output_path, color)
                    print(f"Processed: {input_path} -> {output_path}")
                except Exception as e:
                    print(f"Error processing {input_path}: {str(e)}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Convert images in directory to specified color while preserving transparency')
    parser.add_argument('input', help='Input directory')
    parser.add_argument('--output', help='Output directory (optional)', default=None)
    parser.add_argument('--color', nargs=3, type=int, default=[255, 255, 255],
                        help='RGB color values (0-255), e.g., --color 255 255 255 for white')

    args = parser.parse_args()
    process_directory(args.input, args.output, tuple(args.color))