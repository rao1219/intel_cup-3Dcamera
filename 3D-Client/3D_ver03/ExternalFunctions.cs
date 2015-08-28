using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using SharpGL;
using WPFCustomMessageBox;

namespace _3D_ver03
{
    unsafe class ExternalFunctions
    {
        static int isdeviced;
        static int isreinitlibs;
        static int iswarned;
        static public float[] infos;
        //static public int maxdistance;
        static private int m_validJointPoint;
        static private int m_width, m_height, m_mapx, m_mapy;

        static float rat = 0.025f;


        static float[] ambient0 = { 0.6f, 0.6f, 0.6f, 1f };
        static float[] position0 = { 3f, 0f, 1f, 1f };
        static float[] diffuse0 = { 0.8f, 0.4f, 0f, 1f };
        static float[] specular0 = { 0.4f, 0.4f, 0.4f, 1f };

        static float[] ambient1 = { 0.4f, 0.4f, 0.4f, 1f };
        static float[] position1 = { 0f, 0f, 5f, 1f };
        static float[] diffuse1 = { 0.4f, 0.6f, 0.4f, 1f };
        static float[] specular1 = { 0.2f, 0.2f, 0.2f, 1f };

        static float[] ambient2 = { 0.5f, 0.5f, 0.5f, 1f };
        static float[] position2 = { 0f, 0f, 5f, 1f };
        static float[] diffuse2 = { 0.3f, 0.5f, 0.3f, 1f };
        static float[] specular2 = { 0.0f, 0.0f, 0.0f, 1f };
        static int _init = 0;
        static int env = 0;

        public static void InitGraphEnv(int wid, int hei)
        {
            if (wid == 0 || hei == 0) return;
            if (_init == 1) return;
            _init = 1;
            glLightfv(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambient0);
            glLightfv(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, position0);
            glLightfv(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, diffuse0);
            glLightfv(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, specular0);

            glLightfv(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambient1);
            glLightfv(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, position1);
            glLightfv(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, diffuse1);
            glLightfv(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, specular1);

            glLightfv(OpenGL.GL_LIGHT2, OpenGL.GL_AMBIENT, ambient2);
            glLightfv(OpenGL.GL_LIGHT2, OpenGL.GL_POSITION, position2);
            glLightfv(OpenGL.GL_LIGHT2, OpenGL.GL_DIFFUSE, diffuse2);
            glLightfv(OpenGL.GL_LIGHT2, OpenGL.GL_SPECULAR, specular2);


        }
        void InitPixel()
        {

        }
        public static void SetWindowsSize(int wid, int hei)
        {
            m_width = wid;
            m_height = hei;
            InitGraphEnv(wid, hei);
        }
        public ExternalFunctions()
        {
            infos = new float[GetInfosSize()];
            isdeviced = ExternalFunctions.InitEnveriments();
            isreinitlibs = 0;
            iswarned = 0;
            isreinitlibs = 0;
        }
        public static void OnPaint()
        {
            if (env == 0)
            {
                //int handle = GetModuleHandleA(null);
                //int hdc = GetDC(handle);
                //env=InitPixelEnv(hdc)?1:0;
                //int argc = 1;
                //char** argv=null;
                ////char* a = "qwe";
                //*argv = a;
                //glutInit(&argc,argv);
            }
            if (isreinitlibs == 1 && isdeviced == 0)
            {
                isdeviced = ReInitLibs();
                iswarned = 0;
                isreinitlibs = 0;
            }
            if (isdeviced == 0)
            {
                if (iswarned == 0)
                {
                    MessageBoxResult result = CustomMessageBox.ShowYesNoCancel("未找到摄像头！", "错误", "重新连接", "退出", "取消");
                    if (result == MessageBoxResult.Yes)
                    {
                        isreinitlibs = 1;
                    }
                    else if (result == MessageBoxResult.No)
                        App.Current.Shutdown();
                    else
                        iswarned = 1;
                }
                return;
            }
            if (GetIsInit() == 0)
                InitGraph();

            try
            {
                InitDisplayMode();
                RawDataDeal();
                JointFun();
                DataFun();
            }
            catch
            {
                MessageBox.Show("超出范围！程序自动退出！");
                App.Current.Shutdown();
            }

            GetInfos(infos);

            if (GetConfigStatus(3) == 1)
            {
                m_mapx = GetMapX();
                m_mapy = GetMapY();
                DrawTexture();
                DrawJointSphere();
                DrawModel();
                DrawChart();
            }
            else
            {
                glClear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            }
            RefRelease();
			EventFun();
            glFlush();
            uint err = glGetError();
            //if (err != 0) CustomMessageBox.ShowYesNoCancel("OPENGL", "错误", "重连", "退出", "取消"); ;
        }

        static void DrawTexture()
        {
            float[] x = new float[12];
            float[] y = new float[12];
            float[] z = new float[12];
            GetCoorX(x);
            GetCoorY(y);
            GetCoorZ(z);
            GenerateTex();  //May cause exception
            glClear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            glMatrixMode(OpenGL.GL_PROJECTION);
            glLoadIdentity();
            glOrtho(0, m_width, m_height, 0, 100, -100);
            glMatrixMode(OpenGL.GL_MODELVIEW);
            glViewport(0, 0, (int)(m_width * 1), (int)m_height);
            glLoadIdentity();
            gluLookAt(0, 0, 10, 0, 0, 0, 0, 1, 0);
            glColor3f(1f, 1f, 1f);
            byte[] tex = new byte[m_mapx * m_mapy * 3];
            byte[] textemp = new byte[256 * 256 * 3];
            GetTexData(tex);
            uint[] texarr = new uint[1];
            glGenTextures(1, texarr);
            glBindTexture(OpenGL.GL_TEXTURE_2D, texarr[0]);
            glTexEnvf(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            ScaleImage(m_mapx, m_mapy, tex, 256, 256, textemp);
            glTexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, 256, 256, 0, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, textemp);
            glTexParameteri(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            glTexParameteri(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            glEnable(OpenGL.GL_TEXTURE_2D);
            glDisable(OpenGL.GL_DEPTH_TEST);
            glBegin(OpenGL.GL_QUADS);
            glTexCoord2f(0f, 0f); glVertex3f(0f, 0f, 0f);
            glTexCoord2f(1f, 0f); glVertex3f((float)m_width, 0f, 0f);
            glTexCoord2f(1f, 1f); glVertex3f(m_width, m_height, 0f);
            glTexCoord2f(0f, 1f); glVertex3f(0f, m_height, 0f);
            glEnd();
            glDisable(OpenGL.GL_TEXTURE_2D);
            glColor3f(1f, 0, 1f);
            for (int i = 0; i < 2; i++)
            {
                glPushMatrix();
                glTranslatef(GetJointData(i, 0) / GetMapX() * m_width, GetJointData(i, 1) / GetMapY() * m_height, 0);
                glPointSize(10);
                glBegin(OpenGL.GL_POINTS);
                glVertex3f(0f, 0f, 0f);
                glEnd();
                glPointSize(1);
                glPopMatrix();
            }
            glDeleteTextures(1, texarr);
        }

        static void DrawJointSphere()
        {
            if (GetIsTracked() == 0) return;
            m_validJointPoint = GetJointSize();
            float[] cood = new float[3 * m_validJointPoint];
            for (int i = 0; i < m_validJointPoint; i++)
            {
                ConvertToScreen(i, ref cood[3 * i], ref cood[3 * i + 1]);
                cood[3 * i + 0] *= (float)(m_width * 1.0 / GetMapX());
                cood[3 * i + 1] *= (float)(m_height * 1.0 / GetMapY());
                cood[3 * i + 2] *= 0;
            }
            glColor3f(1f, 0.5f, 0f);
            glEnable(OpenGL.GL_CULL_FACE);
            glPolygonMode(OpenGL.GL_FRONT, OpenGL.GL_FILL);
            glCullFace(OpenGL.GL_BACK);
            for (int i = 1; i < m_validJointPoint; i++)
            {
                if (i == 3 || i == 9 || i == 4 || i == 5)
                {
                    glPushMatrix();
                    glLoadIdentity();
                    glTranslatef(cood[i * 3], cood[i * 3 + 1], cood[i * 3 + 2]);
                    glPointSize(10);
                    glBegin(OpenGL.GL_POINTS);
                    glVertex3f(0, 0, 0);
                    glEnd();
                    glPointSize(1);
                    glPopMatrix();
                }
            }
            glDisable(OpenGL.GL_CULL_FACE);
        }

        static void DrawModel()
        {
            float[] x = new float[12];
            float[] y = new float[12];
            float[] z = new float[12];
            GetCoorX(x);
            GetCoorY(y);
            GetCoorZ(z);
            int init = 0;
            float sx = 0, sy = 0, sz = 0;
            float angle = 0;
            int ratio = 100;
            float mx1 = x[9] / ratio, mx2 = x[1] / ratio, mx3 = x[2] / ratio, mx4 = x[3] / ratio;
            float my1 = y[9] / ratio, my2 = y[1] / ratio, my3 = y[2] / ratio, my4 = y[3] / ratio;
            float mz1 = z[9] / ratio, mz2 = z[1] / ratio, mz3 = z[2] / ratio, mz4 = z[3] / ratio;
            glEnable(OpenGL.GL_SCISSOR_TEST);
            glScissor((int)(m_width * 0.75), (int)(m_height * 0.75), m_width, m_height);
            glColor4f(0f, 0f, 0f, 0f);
            glClear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            glEnable(OpenGL.GL_DEPTH_TEST);
            glMatrixMode(OpenGL.GL_PROJECTION);
            glLoadIdentity();
            gluPerspective(45, m_width * 1.0 / m_height, 0.01, 1000);
            glMatrixMode(OpenGL.GL_MODELVIEW);
            glLoadIdentity();
            glViewport((int)(m_width * 0.75), (int)(m_height * 0.75), (int)(m_width * 0.25), (int)(m_height * 0.25));

            glEnable(OpenGL.GL_LIGHTING);
            glEnable(OpenGL.GL_LIGHT0);
            glEnable(OpenGL.GL_LIGHT1);
            glEnable(OpenGL.GL_LIGHT2);
            glEnable(OpenGL.GL_CULL_FACE);
            glCullFace(OpenGL.GL_BACK);
            glPolygonMode(OpenGL.GL_FRONT, OpenGL.GL_FILL);
            glShadeModel(OpenGL.GL_SMOOTH);
            position2[0] = (float)(sx + 5 * Math.Cos(angle / 180 * 3.14));
            position2[1] = sy + 0;
            position2[2] = (float)(sz + 5 * Math.Sin(angle / 180 * 3.14));
            angle += 5;
            glLightfv(OpenGL.GL_LIGHT2, OpenGL.GL_POSITION, position2);
            glLightfv(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, position0);
            glLightfv(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, position1);
            if (GetIsTracked() == 0 || GetIsStart() == 0)
            {
                sx = sy = sz = 0;
                init = 0;
                gluLookAt(0, 0, 5, 0, 0, 0, 0, 1, 0);
                glPushMatrix();
                glPointSize(10);
                glBegin(OpenGL.GL_POINTS);
                glVertex3f(0, 0, 0);
                glEnd();
                glPointSize(1);
                glPopMatrix();
            }
            else
            {
                if (init == 0)
                {
                    sx = mx4;
                    sy = my4;
                    sz = mz4;
                    position0[0] = sx + 5;
                    position0[1] = sy;
                    position0[2] = sz;
                    position1[0] = sx;
                    position1[1] = sy;
                    position1[2] = sz + 5;
                    glLightfv(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, position0);
                    glLightfv(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, position1);
                    init = 1;
                }

                double distance = Math.Sqrt((sx - mx4) * (sx - mx4) + (sy - my4) * (sy - my4) + (sz - mz4) * (sz - mz4));
                if (distance >= 8)
                {
                    sx = (float)(sx - (mx4 - sx) / distance * 0.1);
                    sy = (float)(sy - (my4 - sy) / distance * 0.1);
                    sz = (float)(sz - (mz4 - sz) / distance * 0.1);
                }
                double distance2 = Math.Sqrt((sz - mz4) * (sz - mz4));
                if (distance2 <= 2 && distance2 > 0.1)
                    sz = (float)(sz + (mz4 - sz) / distance2 * 0.1);
                double distance3 = Math.Sqrt((sx - mx4) * (sx - mx4) + (sy - my4) * (sy - my4));
                if (distance3 >= 2)
                {
                    sx = (float)(sx + (mx4 - sx) / distance3 * 0.1);
                    sy = (float)(sy + (my4 - sy) / distance3 * 0.1);
                }

                gluLookAt((double)(sx + 7 * Math.Cos(45.0 / 180 * 3.14)), (double)sy, (double)(sz + 7 * Math.Sin(45.0 / 180 * 3.14)), (double)sx, (double)sy, (double)sz, 0, 1, 0);
                ////////////head//////////////
                glPushMatrix();
                glTranslatef(mx1, my1, mz1);
                glPointSize(10);
                glBegin(OpenGL.GL_POINTS);
                glVertex3f(0, 0, 0);
                glEnd();
                glPointSize(1);
                glPopMatrix();
                ///////////left-shoulder/////
                glPushMatrix();
                glTranslatef(mx2, my2, mz2);
                glPointSize(10);
                glBegin(OpenGL.GL_POINTS);
                glVertex3f(0, 0, 0);
                glEnd();
                glPointSize(1);
                glPopMatrix();
                //////////righr-shoulder////
                glPushMatrix();
                glTranslatef(mx3, my3, mz3);
                glPointSize(10);
                glBegin(OpenGL.GL_POINTS);
                glVertex3f(0, 0, 0);
                glEnd();
                glPointSize(1);
                glPopMatrix();
                //////////neck/////////////
                glPushMatrix();
                glTranslatef(mx4, my4, mz4);
                glPointSize(10);
                glBegin(OpenGL.GL_POINTS);
                glVertex3f(0, 0, 0);
                glEnd();
                glPointSize(1);
                glPopMatrix();
                //////////line-to-joint////

                glLineWidth(2.5f);
                float[] mambient0 = { 0.3f, 0.3f, 0.3f, 0.5f };
                glPushAttrib(OpenGL.GL_LIGHTING_BIT);
                glBegin(OpenGL.GL_LINES);
                glMaterialfv(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, mambient0);
                glVertex3f(mx4, my4, mz4);
                glVertex3f(mx1, my1, mz1);
                glVertex3f(mx4, my4, mz4);
                glVertex3f(mx2, my2, mz2);
                glVertex3f(mx4, my4, mz4);
                glVertex3f(mx3, my3, mz3);
                glEnd();
                glPopAttrib();
            }
            glDisable(OpenGL.GL_LIGHTING);
            glDisable(OpenGL.GL_CULL_FACE);
            glDisable(OpenGL.GL_SCISSOR_TEST);
        }

        static void DrawChart()
        {
            int scx = (int)(m_width * 0.75), scy = 0;
            int scwid = (int)(m_width * 0.25), schei = (int)(m_height * 0.75);

            glEnable(OpenGL.GL_SCISSOR_TEST);
            glScissor(scx, scy, scwid, schei);
            glColor3f(0, 0, 0);
            glClear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            glEnable(OpenGL.GL_BLEND);
            glBlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            glMatrixMode(OpenGL.GL_PROJECTION);
            glLoadIdentity();
            glOrtho(0, scwid, 0, schei, 100, -100);
            glMatrixMode(OpenGL.GL_MODELVIEW);
            glLoadIdentity();
            glViewport(scx, scy, scwid, schei);
            gluLookAt(0, 0, 100, 0, 0, 0, 0, 1, 0);
            glEnable(OpenGL.GL_DEPTH_TEST);
            glLineWidth(1);
            double tempx = m_width * 0.25;
            double tempy = m_height * 0.75;
            glBegin(OpenGL.GL_LINES);
            glColor4f(0.7f, 0.7f, 0.7f, 0.4f);
            for (float x = 0; x <= tempx + 0.001; x += m_width * rat)
            {
                glVertex3f(x, 0, 0);
                glVertex3f(x, (float)tempy, 0);
            }
            for (float y = 0; y <= tempy + 0.001; y += m_height * rat)
            {
                glVertex3f(0, y, 0);
                glVertex3f((float)tempx, y, 0);
            }
            glEnd();
            glLineWidth(3);
            glColor4f(0.5f, 0.4f, 1f, 0.4f);
            glBegin(OpenGL.GL_LINES);
            glVertex3f(0f, (float)(m_height * 0.375), 0f);
            glVertex3f((float)(m_width * 0.25), (float)(m_height * 0.375), 0f);
            glEnd();
            glLineWidth(1.5f);

            /////////////////////////std-data//////////////////////
            glColor4f(1f, 0.6f, 0f, 1f);
            float drawx, drawy;
            drawx = (float)(m_width * 0.075);
            drawy = (float)(m_height * 0.375);
            glBegin(OpenGL.GL_LINE_STRIP);
            for (int i = GetStdData2_index() - 1; i >= 0 && drawx >= -0.001; i--)
            {
                float drtoy = (float)(GetStdData2_at((uint)i) / 10 * m_height * 0.025 + drawy);
                glVertex3f(drawx, drtoy, 1);
                drawx -= (float)(m_width * 0.025);
            }
            glEnd();

            /////////////////////per-second/////////////
            glColor4f(0.5f, 1f, 0f, 1f);
            drawx = (float)(m_width * 0.25);
            drawy = (float)(m_height * 0.375);
            glBegin(OpenGL.GL_LINE_STRIP);
            for (int i = GetAngData2_index(); i >= 0 && drawx >= -0.001; i--)
            {
                if (i == 0 && GetAngData2_multi() > 0)
                {
                    i = GetAngData2_size() - 1;
                }
                if (i == 0 && GetAngData2_multi() == 0) break;
                float drtoy = (float)(GetAngData2_at((uint)(i - 1)) / 10 * m_height * 0.025 + drawy);
                glVertex3f(drawx, drtoy, 1);
                drawx -= (float)(m_width * 0.025);
            }
            glEnd();

            /////////////////////per-minter/////////////
            glColor4f(0f, 0.9f, 1f, 1f);
            drawx = (float)(m_width * 0.250);
            drawy = (float)(m_height * 0.375);
            glBegin(OpenGL.GL_LINE_STRIP);
            for (int i = GetAngData3_index(); i >= 0 && drawx >= -0.001; i--)
            {
                if (i == 0 && GetAngData3_multi() > 0)
                {
                    i = GetAngData3_size() - 1;
                }
                if (i == 0 && GetAngData3_multi() == 0) break;
                float drtoy = (float)(GetAngData4_at((uint)(i - 1)) / 10 * m_height * 0.025 + drawy);
                glVertex3f(drawx, drtoy, 1);
                drawx -= (float)(m_width * 0.025);
            }
            glEnd();

            //////////////////////per-hour///////////
            glColor4f(0.9f, 0.45f, 0.9f, 1f);
            drawx = (float)(m_width * 0.25);
            drawy = (float)(m_height * 0.375);
            glBegin(OpenGL.GL_LINE_STRIP);
            for (int i = GetAngData4_index(); i >= 0 && drawx >= -0.001; i--)
            {
                if (i == 0 && GetAngData4_multi() > 0)
                {
                    i = GetAngData4_size() - 1;
                }
                if (i == 0 && GetAngData4_multi() == 0) break;
                float drtoy = (float)(GetAngData4_at((uint)(i - 1)) / 10 * m_height * 0.025 + drawy);
                glVertex3f(drawx, drtoy, 1);
                drawx -= (float)(m_width * 0.025);
            }
            glEnd();

            ////////////////////////diagram///////////////////
            glColor4f(1f, 0.9f, 0.5f, 1f);
            drawx = (float)(m_width * 0.25 / GetAngData2_maxdiagram());
            drawy = 0;
            glBegin(OpenGL.GL_QUADS);
            for (uint i = 0; i < GetAngData2_maxdiagram(); i++)
            {
                drawy = (float)(1.0 * GetAngData2_at(i) / GetAngData2_size() * 0.3 * m_height);
                if (drawy >= 0.3 * m_height)
                {
                    drawy = (float)(0.3 * m_height);
                }
                glVertex3f((float)(i * drawx), 0f, 0.5f);
                glVertex3f((float)((i + 1) * drawx), 0f, 0.5f);
                glVertex3f((float)((i + 1) * drawx), (float)drawy, 0.5f);
                glVertex3f((float)(i * drawx), (float)drawy, 0.5f);
            }
            glEnd();
            /////////////////////////dia for 1 fream///////////
            glColor4f(0.964f, 0.443f, 0.725f, 0.5f);
            drawx = (float)(m_width * 0.25 / GetChart2_maxdiagram());
            drawy = 0;
            glBegin(OpenGL.GL_QUADS);
            for (uint i = 0; i < GetChart2_maxdiagram(); i++)
            {
                drawy = (float)(1.0 * GetChart2_diagram(i) / GetChart2_size() * 0.3 * m_height);
                if (drawy >= 0.3 * m_height)
                {
                    drawy = (float)(0.3 * m_height);
                }
                glVertex3f((float)(i * drawx), 0f, 0.5f);
                glVertex3f((float)((i + 1) * drawx), 0f, 0.5f);
                glVertex3f((float)((i + 1) * drawx), (float)drawy, 0.5f);
                glVertex3f((float)(i * drawx), (float)drawy, 0.5f);
            }
            glEnd();
            glBlendFunc(OpenGL.GL_ONE, OpenGL.GL_ZERO);
            glDisable(OpenGL.GL_BLEND);
            glDisable(OpenGL.GL_DEPTH_TEST);
            glDisable(OpenGL.GL_SCISSOR_TEST);
        }

        #region DllImport
        [DllImport("DCTdui.dll", EntryPoint = "InitEnveriments", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitEnveriments();

        [DllImport("DCTdui.dll", EntryPoint = "ConfigEnable", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConfigEnable(int i);

        [DllImport("DCTdui.dll", EntryPoint = "ConfigDisable", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConfigDisable(int i);

        [DllImport("DCTdui.dll", EntryPoint = "GetConfigAllStatus", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetConfigAllStatus(int[] i);

        [DllImport("DCTdui.dll", EntryPoint = "ScaleImage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ScaleImage(int widthin, int heightin, byte[] indata, int widthout, int heightout, byte[] outdata);

        [DllImport("DCTdui.dll", EntryPoint = "GetJointData", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetJointData(int index, int cood);

        [DllImport("DCTdui.dll", EntryPoint = "GetOutput", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetOutput(char[] o);

        [DllImport("DCTdui.dll", EntryPoint = "GetError", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetError(char[] e);

        [DllImport("DCTdui.dll", EntryPoint = "StartGraph", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StartGraph();

        [DllImport("DCTdui.dll", EntryPoint = "OnKeyDown", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void OnKeyDown(System.UInt32 key);

        [DllImport("DCTdui.dll", EntryPoint = "OnSize", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void OnSize(int x, int y);

        [DllImport("DCTdui.dll", EntryPoint = "GetConfig", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetConfig(double[] con);

        [DllImport("DCTdui.dll", EntryPoint = "GetMaxTime", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetMaxTime(double[] max);

        [DllImport("DCTdui.dll", EntryPoint = "SetConfig", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetConfig(double[] config);

        [DllImport("DCTdui.dll", EntryPoint = "SetMaxTime", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMaxTime(double[] time);

        [DllImport("DCTdui.dll", EntryPoint = "ReInitLibs", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReInitLibs();

        [DllImport("DCTdui.dll", EntryPoint = "GetConfigSize", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetConfigSize();

        [DllImport("DCTdui.dll", EntryPoint = "GetMaxTimeSize", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMaxTimeSize();

        [DllImport("DCTdui.dll", EntryPoint = "InitDisplayMode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitDisplayMode();

        [DllImport("DCTdui.dll", EntryPoint = "DataFun", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DataFun();

        [DllImport("DCTdui.dll", EntryPoint = "InitGraph", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitGraph();

        [DllImport("DCTdui.dll", EntryPoint = "JointFun", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void JointFun();

        [DllImport("DCTdui.dll", EntryPoint = "RawDataDeal", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RawDataDeal();

        [DllImport("DCTdui.dll", EntryPoint = "GenerateTex", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GenerateTex();

        [DllImport("DCTdui.dll", EntryPoint = "ConvertToScreen", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConvertToScreen(int i, ref float ox, ref float oy);

        [DllImport("DCTdui.dll", EntryPoint = "GetJointSize", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetJointSize();

        [DllImport("DCTdui.dll", EntryPoint = "GetIsTracked", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetIsTracked();

        [DllImport("DCTdui.dll", EntryPoint = "GetIsInit", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetIsInit();

        [DllImport("DCTdui.dll", EntryPoint = "GetIsInitGraph", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int* GetIsInitGraph();

        [DllImport("DCTdui.dll", EntryPoint = "GetConfigStatus", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetConfigStatus(int bit);

        [DllImport("DCTdui.dll", EntryPoint = "RefRelease", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RefRelease();

        [DllImport("DCTdui.dll", EntryPoint = "GetCoorX", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCoorX(float[] x);

        [DllImport("DCTdui.dll", EntryPoint = "GetCoorY", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCoorY(float[] y);

        [DllImport("DCTdui.dll", EntryPoint = "GetCoorZ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCoorZ(float[] z);

        //[DllImport("DCTdui.dll", EntryPoint = "GetWidth", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int GetWidth();

        //[DllImport("DCTdui.dll", EntryPoint = "GetHeight", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int GetHeight();

		[DllImport("DCTdui.dll", EntryPoint = "EventFun", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EventFun();

        [DllImport("DCTdui.dll", EntryPoint = "GetMapX", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMapX();

        [DllImport("DCTdui.dll", EntryPoint = "GetMapY", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMapY();

        [DllImport("DCTdui.dll", EntryPoint = "GetTexData", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetTexData(byte[] x);

        [DllImport("DCTdui.dll", EntryPoint = "GetIsStart", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetIsStart();

        [DllImport("DCTdui.dll", EntryPoint = "GetStdData2_index", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetStdData2_index();

        [DllImport("DCTdui.dll", EntryPoint = "GetStdData2_at", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetStdData2_at(uint index);

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData2_index", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAngData2_index();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData2_multi", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAngData2_multi();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData2_size", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAngData2_size();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData2_at", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetAngData2_at(uint index);

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData3_index", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAngData3_index();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData3_multi", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAngData3_multi();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData3_size", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAngData3_size();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData3_at", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetAngData3_at(uint index);

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData4_index", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAngData4_index();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData4_multi", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAngData4_multi();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData4_size", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAngData4_size();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData4_at", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetAngData4_at(uint index);

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData2_maxdiagram", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetAngData2_maxdiagram();

        [DllImport("DCTdui.dll", EntryPoint = "GetAngData2_diagram", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetAngData2_diagram(uint num);

        [DllImport("DCTdui.dll", EntryPoint = "GetChart2_maxdiagram", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetChart2_maxdiagram();

        [DllImport("DCTdui.dll", EntryPoint = "GetChart2_diagram", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetChart2_diagram(uint num);

        [DllImport("DCTdui.dll", EntryPoint = "GetChart2_size", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetChart2_size();

        [DllImport("DCTdui.dll", EntryPoint = "GetMaxDistance", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMaxDistance();

        [DllImport("DCTdui.dll", EntryPoint = "GetInfos", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetInfos(float[] infos);

        [DllImport("DCTdui.dll", EntryPoint = "GetInfosSize", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetInfosSize();

        [DllImport("DCTdui.dll", EntryPoint = "InitPixelEnv", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool InitPixelEnv(int h);

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern int GetDC(int hdc);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleA")]
        public static extern int GetModuleHandleA(char* name);

        [DllImport("opengl32.dll", EntryPoint = "glVertex3f")]
        public static extern void glVertex3f(float a, float b, float c);

        [DllImport("opengl32.dll", EntryPoint = "glClear")]
        public static extern void glClear(System.UInt32 a);

        [DllImport("opengl32.dll", EntryPoint = "glMatrixMode")]
        public static extern void glMatrixMode(System.UInt32 a);

        [DllImport("opengl32.dll", EntryPoint = "glLoadIdentity")]
        public static extern void glLoadIdentity();

        [DllImport("opengl32.dll", EntryPoint = "glOrtho")]
        public static extern void glOrtho(double a, double b, double c, double d, double e, double f);

        [DllImport("opengl32.dll", EntryPoint = "glViewport")]
        public static extern void glViewport(int a, int b, int c, int d);

        [DllImport("glu32.dll", EntryPoint = "gluLookAt")]
        public static extern void gluLookAt(double a, double b, double c, double d, double e, double f, double g, double h, double i);

        [DllImport("opengl32.dll", EntryPoint = "glLightfv")]
        public static extern void glLightfv(System.UInt32 a, System.UInt32 b, float[] c);

        [DllImport("opengl32.dll", EntryPoint = "glFlush")]
        public static extern void glFlush();

        [DllImport("opengl32.dll", EntryPoint = "glEnable")]
        public static extern void glEnable(System.UInt32 a);

        [DllImport("opengl32.dll", EntryPoint = "glColor3f")]
        public static extern void glColor3f(float a, float b, float c);

        [DllImport("opengl32.dll", EntryPoint = "glColor4f")]
        public static extern void glColor4f(float a, float b, float c, float d);

        [DllImport("opengl32.dll", EntryPoint = "glGetError")]
        public static extern System.UInt32 glGetError();

        [DllImport("opengl32.dll", EntryPoint = "glGenTextures")]
        public static extern void glGenTextures(int a, System.UInt32[] b);

        [DllImport("opengl32.dll", EntryPoint = "glBindTexture")]
        public static extern void glBindTexture(System.UInt32 a, System.UInt32 b);

        [DllImport("opengl32.dll", EntryPoint = "glTexParameteri")]
        public static extern void glTexParameteri(System.UInt32 a, System.UInt32 b, System.UInt32 c);

        [DllImport("opengl32.dll", EntryPoint = "glTexEnvf")]
        public static extern void glTexEnvf(System.UInt32 a, System.UInt32 b, float c);

        [DllImport("opengl32.dll", EntryPoint = "glTexImage2D")]
        public static extern void glTexImage2D(System.UInt32 a, int b, System.UInt32 c, int d, int e, int f, System.UInt32 g, System.UInt32 h, byte[] i);

        [DllImport("opengl32.dll", EntryPoint = "glDisable")]
        public static extern void glDisable(System.UInt32 a);

        [DllImport("opengl32.dll", EntryPoint = "glBegin")]
        public static extern void glBegin(System.UInt32 a);

        [DllImport("opengl32.dll", EntryPoint = "glTexCoord2f")]
        public static extern void glTexCoord2f(float a, float b);


        [DllImport("opengl32.dll", EntryPoint = "glEnd")]
        public static extern void glEnd();

        [DllImport("opengl32.dll", EntryPoint = "glPushMatrix")]
        public static extern void glPushMatrix();

        [DllImport("opengl32.dll", EntryPoint = "glTranslatef")]
        public static extern void glTranslatef(float a, float b, float c);

        [DllImport("opengl32.dll", EntryPoint = "glPointSize")]
        public static extern void glPointSize(float a);

        [DllImport("opengl32.dll", EntryPoint = "glPopMatrix")]
        public static extern void glPopMatrix();

        [DllImport("opengl32.dll", EntryPoint = "glDeleteTextures")]
        public static extern void glDeleteTextures(int a, System.UInt32[] b);

        [DllImport("opengl32.dll", EntryPoint = "glPolygonMode")]
        public static extern void glPolygonMode(System.UInt32 a, System.UInt32 b);

        [DllImport("opengl32.dll", EntryPoint = "glCullFace")]
        public static extern void glCullFace(System.UInt32 a);

        [DllImport("opengl32.dll", EntryPoint = "glScissor")]
        public static extern void glScissor(int a, int b, int c, int d);

        [DllImport("glu32.dll", EntryPoint = "gluPerspective")]
        public static extern void gluPerspective(double a, double b, double c, double d);

        [DllImport("opengl32.dll", EntryPoint = "glShadeModel")]
        public static extern void glShadeModel(System.UInt32 a);

        [DllImport("opengl32.dll", EntryPoint = "glLineWidth")]
        public static extern void glLineWidth(float a);

        [DllImport("opengl32.dll", EntryPoint = "glPushAttrib")]
        public static extern void glPushAttrib(System.UInt32 a);

        [DllImport("opengl32.dll", EntryPoint = "glMaterialfv")]
        public static extern void glMaterialfv(System.UInt32 a, System.UInt32 b, float[] c);

        [DllImport("opengl32.dll", EntryPoint = "glPopAttrib")]
        public static extern void glPopAttrib();

        [DllImport("opengl32.dll", EntryPoint = "glBlendFunc")]
        public static extern void glBlendFunc(System.UInt32 a, System.UInt32 b);

        [DllImport("glut32.dll", EntryPoint = "glutInit")]
        public static extern void glutInit(int* argc, char** argv);

        #endregion
    }
}
