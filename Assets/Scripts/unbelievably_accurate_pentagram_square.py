import bpy
import math
import mathutils

def create_cylinder_between_points(start, end, radius=0.1):
    """Create a properly aligned cylinder (sausage) between two points."""
    
    # Compute midpoint
    mid = [(s + e) / 2 for s, e in zip(start, end)]

    # Compute direction vector
    direction = mathutils.Vector((end[0] - start[0], end[1] - start[1], end[2] - start[2]))
    
    # Compute length of the cylinder
    length = direction.length

    # Create cylinder
    bpy.ops.mesh.primitive_cylinder_add(radius=radius, depth=length, location=mid)
    cylinder = bpy.context.object

    # Align cylinder to the direction
    quaternion = direction.to_track_quat('Z', 'Y')  # Align Z-axis along the direction
    cylinder.rotation_euler = quaternion.to_euler()  # Convert to Euler for Blender

def create_outer_circle(radius=1, thickness=0.1):
    """Create a torus to act as the outer ring of the pentagram."""
    major_radius = radius  # Matches the tips of the pentagram
    minor_radius = thickness  # Matches the sausage thickness
    
    bpy.ops.mesh.primitive_torus_add(
        align='WORLD',
        location=(0, 0, 0),  # Centered at origin
        major_radius=major_radius,
        minor_radius=minor_radius,
        major_segments=64,  # Smooth circle
        minor_segments=32
    )

def create_pentagram(radius=1, thickness=0.1):
    """Creates a 3D pentagram with a circular outer ring and correctly rotated sausages."""
    
    # Clear existing mesh objects
    for obj in bpy.context.scene.objects:
        if obj.type == 'MESH':
            bpy.data.objects.remove(obj, do_unlink=True)

    # Compute the 5 main pentagram points in 3D space
    vertices = []
    for i in range(5):
        angle = math.radians(i * 72 - 90)  # Offset by -90 degrees to make the top point upwards
        x = radius * math.cos(angle)
        y = radius * math.sin(angle)
        vertices.append((x, y, 0))  # Keeping it flat on the Z-plane

    # Create the outer circular ring
    create_outer_circle(radius, thickness)

    # Create the inner star connections
    for i in range(5):
        create_cylinder_between_points(vertices[i], vertices[(i + 2) % 5], thickness)

# Call the function to create the properly rotated 3D pentagram with a ring
create_pentagram(radius=2, thickness=0.1)
