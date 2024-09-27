shader_type canvas_item;

uniform vec4 hit_color: source_color;
uniform float blink_intensity = 0;

void fragment() {
	vec4 color = texture(TEXTURE, UV);
	color = mix(color, hit_color, blink_intensity * color.a);
	COLOR = color;
}
