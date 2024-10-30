shader_type canvas_item;

uniform vec4 water_color:source_color;
uniform sampler2D fast_noise:repeat_enable;
uniform sampler2D screen_content:hint_screen_texture;
uniform vec2 speed = vec2(0.03, 0.07);
uniform float wave_strength = 2;

void fragment() {
	vec2 uv = UV + speed * TIME;
	vec2 noise = texture(fast_noise, uv).rg;
	
	COLOR = vec4(water_color.rgb, 1) + vec4(noise.xy, 1, 1);
	COLOR *= texture(screen_content, SCREEN_UV + noise * (wave_strength / 100.0));
}