#version 330

in vec2 fragTexCoord;
out vec4 finalColor;

uniform sampler2D u_screenTexture;
uniform vec2 u_mousePosition = vec2(960.0, 540.0);
uniform vec2 u_textureSize = vec2(1920.0, 1080.0);
uniform float u_radius = 50.0;
uniform float u_zoom = 2.0;
uniform float u_gridAlpha = 0.125;
uniform float u_gridThickness = 0.05;

void main()
{
    vec2 texelPos = fragTexCoord * u_textureSize;

    // sample position
    float dist = distance(texelPos, u_mousePosition);
    float mask = step(dist, u_radius);
    vec2 dir = texelPos - u_mousePosition;
    vec2 samplePos = mix(texelPos, u_mousePosition + dir / u_zoom, mask);

    vec2 uv = samplePos / u_textureSize;
    vec4 color = texture(u_screenTexture, uv);

    // grid
    bool magnified = mask > 0;
    if (magnified && u_zoom >= 4.0) {
        vec2 cell = abs(fract(samplePos - 0.5) - 0.5) / fwidth(samplePos);

        float line = min(cell.x, cell.y);
        float grid = 1.0 - min(line, 1.0);

        color.rgb = mix(color.rgb, vec3(0.0), grid * u_gridAlpha);
    }

    finalColor = color;
}
