#include <windows.h>
#include <GL\glut.h>
#include <stdio.h>
#include <time.h>
#include <vector>
#define _USE_MATH_DEFINES
#include <math.h>
#include <fstream>
#include <iostream>

typedef struct
{
	float x, y, z;
} Vec3;

typedef struct 
{
	float r, g, b;
} Color;

typedef struct
{
	Vec3 pos;
	float intensity;
} Voxel;

std::vector<Voxel> voxels;
std::vector< std::vector<int> > arcs;
std::vector< std::vector<float> > arclengths;

Vec3 centerOfMass = {};

int winWidth = 600, winHeight = 600;

float r = 50.0;

GLfloat translate_x=.0, 
		translate_y=.0, 
		translate_z=.0;

GLfloat rotate_x = 0.0,
		rotate_y = 0.0;

GLdouble eyeX=.0, eyeY=.0, eyeZ=50.0, upX=.0, upY=1.0, upZ=.0;

float min_intensity, max_intensity, min_arclength, max_arclength;

bool isMouseDown = false;

int mouse_x = 0, mouse_y = 0;


void calculateMinMaxIntensity()
{
	min_intensity = voxels[0].intensity, max_intensity = voxels[0].intensity;
	for (std::vector<int>::size_type i = 0; i != voxels.size(); i++)
	{
		if (voxels[i].intensity < min_intensity)
		{
			min_intensity = voxels[i].intensity;
		}
		if (voxels[i].intensity > max_intensity)
		{
			max_intensity = voxels[i].intensity;
		}
	}
}

void calculateMinMaxArclength()
{
	min_arclength = arclengths[0][0], max_arclength = arclengths[0][0];
	for (std::vector<int>::size_type i = 0; i != arclengths.size(); i++)
	{
		for (std::vector<int>::size_type j = 0; j != arclengths[i].size(); j++)
		{
			if (arclengths[i][j] < min_arclength)
			{
				min_arclength = arclengths[i][j];
			}
			if (arclengths[i][j] > max_arclength)
			{
				max_arclength = arclengths[i][j];
			}
		}
	}
}

void importVoxels()
{
	std::ifstream file("voxels.txt");

	int x, y, z;
	while ((file >> x) && (file >> y) && (file >> z))
	{
		Voxel voxel = {};
		Vec3 pos = { x, y, z };
		voxel.pos = pos;

		voxels.push_back(voxel);

		centerOfMass.x += x;
		centerOfMass.y += y;
		centerOfMass.z += z;
	}
	centerOfMass.x /= voxels.size();
	centerOfMass.y /= voxels.size();
	centerOfMass.z /= voxels.size();
}

void importIntensities()
{
	std::ifstream file("intensity.txt");

	float x;
	int i = 0;
	while (file >> x)
	{
		voxels[i].intensity = x;
		i++;
	}

	calculateMinMaxIntensity();
}

void importArcs()
{
	std::ifstream neighbors_file("neighbors.txt");
	std::ifstream neighborhood_file("neighborhood.txt");
	std::ifstream arclengths_file("arclengths.txt");

	int numberOfNeighbors;
	while (neighborhood_file >> numberOfNeighbors)
	{
		int neighbor;
		float arclength;
		std::vector<int> _arcs;
		std::vector<float> _arclengths;
		for (int i = 0; i < numberOfNeighbors; i++)
		{
 			neighbors_file >> neighbor;
			_arcs.push_back(neighbor);

			arclengths_file >> arclength;
			_arclengths.push_back(arclength);


		}
		arcs.push_back(_arcs);
		arclengths.push_back(_arclengths);
	}

	calculateMinMaxArclength();
}

void initScene() 
{
	importVoxels();
	importIntensities();
	importArcs();

	glClearColor(0.0, 0.0, 0.0, 0.0);
	glEnable(GL_DEPTH_TEST);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	gluPerspective(80, 1, 1, 1000);
	glMatrixMode(GL_MODELVIEW);
}

void drawCube(Color c)
{
	glScalef(.2, .2, .2);
	glBegin(GL_QUADS);
	//front face
	glColor3f(c.r, c.g, c.b);
	glVertex3f(-1, -1, 1);
	glVertex3f(1, -1, 1);
	glVertex3f(1, 1, 1);
	glVertex3f(-1, 1, 1);

	//back face
	glColor3f(0.6*c.r, 0.6*c.g, 0.6*c.b);
	glVertex3f(-1, 1, -1);
	glVertex3f(1, 1, -1);
	glVertex3f(1, -1, -1);
	glVertex3f(-1, -1, -1);

	//top face        
	glColor3f(1.2*c.r, 1.2*c.g, 1.2*c.b);
	glVertex3f(-1, 1, 1);
	glVertex3f(1, 1, 1);
	glVertex3f(1, 1, -1);
	glVertex3f(-1, 1, -1);

	//bottom face
	glColor3f(0.6*c.r, 0.6*c.g, 0.6*c.b);
	glVertex3f(-1, -1, -1);
	glVertex3f(1, -1, -1);
	glVertex3f(1, -1, 1);
	glVertex3f(-1, -1, 1);

	// left face
	glColor3f(0.8*c.r, 0.8*c.g, 0.8*c.b);
	glVertex3f(1, -1, 1);
	glVertex3f(1, -1, -1);
	glVertex3f(1, 1, -1);
	glVertex3f(1, 1, 1);

	// right face        
	glColor3f(0.8*c.r, 0.8*c.g, 0.8*c.b);
	glVertex3f(-1, 1, 1);
	glVertex3f(-1, 1, -1);
	glVertex3f(-1, -1, -1);
	glVertex3f(-1, -1, 1);
	glEnd();
}

Color calculateColor(float intensity, float min, float max)
{
	Color c = {};

	float val = (intensity - min) / (max - min);

	if (val >= 0 && val < 0.25)
	{
		c = { 1.0, val * 4, 0.0 };
	}
	else if (val >= 0.25 && val < 0.50)
	{
		c = { 1.0 - (val - 0.25) * 4, 1.0, 0.0 };
	}
	else if (val >= 0.50 && val < 0.75)
	{
		c = { 0.0, 1.0, (val - 0.50) * 4 };
	}
	else if (val >= 0.75 && val < 1.00)
	{
		c = { 0.0, 1.0 - (val - 0.75) * 4, 1.0 };
	}

	return c;
}

void drawVoxels()
{
	for (std::vector<int>::size_type i = 0; i != voxels.size(); i++) 
	{
		glPushMatrix();
		glTranslatef(voxels[i].pos.x - centerOfMass.x, 
					 voxels[i].pos.y - centerOfMass.y, 
					 voxels[i].pos.z - centerOfMass.z);
		//glScalef(0.3,.3,.3);
		Color c = calculateColor(voxels[i].intensity, min_intensity, max_intensity);
		//glColor3f(c.r, c.g, c.b);
		//glutSolidSphere(1, 16, 16);
		drawCube(c);
		glPopMatrix();
	}
}

void drawArcs()
{
	glPushMatrix();
	glEnable(GL_LINE_SMOOTH);
	glLineWidth(0.1);

	for (std::vector<int>::size_type voxel = 0; voxel != arcs.size(); voxel++)
	{
		for (std::vector<int>::size_type i = 0; i != arcs[voxel].size(); i++)
		{
			int neighbor = arcs[voxel][i] - 1;
			glBegin(GL_LINES);

			Color c = calculateColor(arclengths[voxel][i], min_arclength, max_arclength);
			
			glColor4f(c.r, c.g, c.b, 1.0);
			glVertex3f( voxels[voxel].pos.x - centerOfMass.x,
						voxels[voxel].pos.y - centerOfMass.y,
						voxels[voxel].pos.z - centerOfMass.z);
			glVertex3f( voxels[neighbor].pos.x - centerOfMass.x,
						voxels[neighbor].pos.y - centerOfMass.y,
						voxels[neighbor].pos.z - centerOfMass.z);

			glEnd();
		}
	}
	glPopMatrix();
}

void renderScene() 
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glLoadIdentity();
	gluLookAt(eyeX, eyeY, eyeZ,
			  .0,.0,.0,
			  upX, upY, upZ);

	glTranslatef(translate_x, translate_y, translate_z);
	glRotatef(rotate_y, 0, 1, 0);
	glRotatef(rotate_x, 1, 0, 0);

	drawVoxels();
	drawArcs();
	
	glutSwapBuffers();
}

/*-------------------
Keyboard
-------------------*/
void processKeyboard(unsigned char key, int x, int y) {
	switch (key)
	{
	case 'a':
		translate_x -= .2;
		break;
	case 's':
		translate_y -= .2;
		break;
	case 'd':
		translate_x += .2;
		break;
	case 'w':
		translate_y += .2;
		break;
	case 'e':
		translate_z -= .2;
		break;
	case 'r':
		translate_z += .2;
		break;
	}	

	glutPostRedisplay();
}

/*-------------------
Motion
-------------------*/
void processMotion(int x, int y)
{
	if (isMouseDown) {
		rotate_x += (y - mouse_y) / 2;
		rotate_y += (x - mouse_x) / 2;
		mouse_x = x;
		mouse_y = y;
		glutPostRedisplay();
	}
}

/*-------------------
Mouse
-------------------*/
void processMouse(int button, int state, int x, int y)
{
	if (button == GLUT_LEFT_BUTTON) {
		if (state == GLUT_DOWN) {
			isMouseDown = true;
			mouse_x = x;
			mouse_y = y;
		}
		else if (state == GLUT_UP) {
			isMouseDown = false;
		}
	}
//	glutPostRedisplay();
}

void SpecialKeys(int key, int x, int y) {
	if (key == GLUT_KEY_UP)		r -= 0.1;
	if (key == GLUT_KEY_DOWN)	r += 0.1;

	processMotion(x,y);
}

/*-------------------
Idle
-------------------*/
void processIdle()
{
//	glutPostRedisplay();
}

/*---------
Main
---------*/
int main(int argc, char **argv)
{
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DEPTH | GLUT_DOUBLE | GLUT_RGBA);
	glutInitWindowPosition(10, 10);
	glutInitWindowSize(winWidth, winHeight);
	glutCreateWindow("Basic Viewer");

	glutKeyboardFunc(processKeyboard);
	glutMotionFunc(processMotion);
	glutMouseFunc(processMouse);
	glutSpecialFunc(SpecialKeys);
	glutDisplayFunc(renderScene);
	glutIdleFunc(processIdle);
	
	initScene();

	glutMainLoop();

	return(0);
}