shader_type canvas_item;

uniform float intensity: hint_range(0.0, 20.0, 0.1);
uniform float mask: hint_range(0.0, 1.0, 0.01);
uniform float speed: hint_range(0.0, 20.0, 0.01);

vec2 rotate(vec2 p, float angle){
	return vec2(cos(angle) * p.x - sin(angle) * p.y, sin(angle) * p.x + cos(angle) * p.y);
}

void fragment() {
	vec2 uv = UV;
	float c = length(UV - vec2(0.5));
	uv = rotate(uv - vec2(0.5), intensity * c) + vec2(0.5);
	uv.x = abs((uv.x - 0.5) * 2.0);
	uv.x = fract(uv.x + speed * TIME);
	vec4 tex_color = texture(TEXTURE, uv);
	COLOR = tex_color;
	float mask_color = step(c, mask);
	COLOR.a = COLOR.a * mask_color;
	c = 1.5 - c * 3.0;
	COLOR.a = COLOR.a * c;
}
