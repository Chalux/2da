shader_type canvas_item;
uniform bool enable = false;
uniform float scan_strength = 3600.0;

float remap(float x, float t1, float t2, float s1, float s2) {
	return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
}

void fragment() {
	if (enable) {
		float source_value = sin(UV.y * scan_strength + 20.0 * TIME);
		float temp_value = remap(sin(5.0 * TIME), 0.0, 1.0, 0.2, 0.7);
		COLOR.a = mix(0, remap(source_value, 0.0, 1.0, temp_value, 1.0), texture(TEXTURE, UV).a);
	}
}