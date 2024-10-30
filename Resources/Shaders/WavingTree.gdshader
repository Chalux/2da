shader_type canvas_item;
uniform float WindStrength:hint_range(2.0, 10.0, 0.1);

void vertex() {
	//VERTEX = vec2(sin(TIME + VERTEX.x) * VERTEX.x * (1.0 - UV.y), VERTEX.y);
	//VERTEX = vec2(VERTEX.x * (WindStrength * (1.0 - UV.y) * sin(VERTEX.x + TIME)), VERTEX.y);
	VERTEX = vec2(VERTEX.x + ((WindStrength * (1.0 - UV.y)) * sin(VERTEX.x + TIME)), VERTEX.y);
}

void fragment() {
	// Called for every pixel the material is visible on.
}