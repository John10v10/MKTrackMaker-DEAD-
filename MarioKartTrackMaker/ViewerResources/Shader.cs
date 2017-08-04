using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace MarioKartTrackMaker.ViewerResources
{
    class Shader
    {
        public static int CompileFragmentShader()
        {
            int s;

            s = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(s, @"
#version 120

uniform sampler2D texture;
uniform int useTexture;
uniform vec3 scale;
varying vec3 vPosition;
varying vec3 vNormal;
varying vec3 LightPos1;
varying vec3 LightPos2;
varying vec2 vUVs;
varying vec4 vColor;
void main(){
    vec3 Light1 = vec3(1)*pow(max(dot(normalize(vNormal/scale), normalize(LightPos1))/2+0.5,0.125),2);
    vec3 Light2 = vec3(0.95,0.94,1)*pow(max(dot(normalize(vNormal/scale), normalize(LightPos2))+0.625,0),2)*1.1;
	vec3 texmap = vColor.xyz;
    if(useTexture == 1)texmap = texture2D(texture,vUVs.xy).xyz;
    gl_FragColor = vec4((Light1+Light2)*texmap,1);
}
"
);
            GL.CompileShader(s);
            string stats;
            GL.GetShaderInfoLog(s, out stats);
            Console.WriteLine(stats);
            return s;
        }
        public static int CompileVertexShader()
        {
            int s;

            s = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(s, @"
#version 120
varying vec3 vPosition;
varying vec3 vNormal;
varying vec2 vUVs;
varying vec3 LightPos1;
varying vec3 LightPos2;
varying vec4 vColor;
void main(){
	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
	vPosition = vec3(gl_ModelViewMatrix * gl_Vertex);
	vColor = gl_Color;
    LightPos1 = normalize(transpose(mat3(gl_ModelViewMatrix))*vec3(1,1,-4));
    LightPos2 = normalize(transpose(mat3(gl_ModelViewMatrix))*vec3(-1,-1,6));
	vNormal = gl_Normal;
    vUVs = gl_MultiTexCoord0.st;
}
"
);
            GL.CompileShader(s);
            string stats;
            GL.GetShaderInfoLog(s, out stats);
            Console.WriteLine(stats);
            return s;
        }
        public static int ProgramLink(int vs, int fs)
        {
            int pgm = GL.CreateProgram();
            GL.AttachShader(pgm, vs);
            GL.AttachShader(pgm, fs);
            GL.LinkProgram(pgm);
            string stats;
            GL.GetProgramInfoLog(pgm, out stats);
            Console.WriteLine(stats);
            return pgm;
        }
    }
}
