shader_type canvas_item;
uniform vec4 blink_color: source_color;
uniform float blink_intensity:hint_range(0.0, 1.0, 0.01) = 0;

void fragment() {
	vec4 color = texture(TEXTURE, UV);
	color.rgb = mix(color.rgb, blink_color.rgb, blink_intensity);
	
	COLOR = color;
}
