using Ciloci.Flee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphing3D
{
    unsafe class Plotting3D
    {
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plinit();
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void plMinMax2dGrid(double** z, int nx, int ny, ref double fmax, ref double fmin);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plscmap1n(int ncol1);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plscmap1l(int itype, int npts, double[] intensity,
             double[] coord1, double[] coord2, double[] coord3, int[] alt_hue_path);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_pladv(int page);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plcol0(int icol0);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plvpor(double xmin, double xmax, double ymin, double ymax);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plwind(double xmin, double xmax, double ymin, double ymax);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plw3d(double basex, double basey, double height, double xmin, double xmax,
                                                            double ymin, double ymax, double zmin, double zmax, double alt, double az);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plbox3(string xopt, string xlabel, double xtick, int nxsub,
                                                              string yopt, string ylabel, double ytick, int nysub,
                                                              string zopt, string zlabel, double ztick, int nzsub);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plmeshc(double[] x, double[] y, double** z, int nx, int ny, int opt, double[] clevel, int nlevel);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plmtex(string side, double disp, double pos, double just, string text);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plend();
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void c_plsdev(string devname);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int plAlloc2dGrid(ref double** f, int nx, int ny);
        [DllImport("C:\\Users\\San\\Desktop\\plplot-5.13.0\\Build_New\\dll\\Debug\\plplot.dll", CallingConvention = CallingConvention.Cdecl)]
       protected static extern int c_plsfnam(String fnam);
       

        const int LEVELS = 10;
        const int MAG_COLOR = 0x004;// draw lines parallel to both the X and Y axis                               
        const int BASE_CONT = 0x008; // draw the mesh with a color dependent of the magnitude                         
        const int DRAW_LINEXY = 0x003;    // draw contour plot at bottom xy plane

        private int XPTS;          // so diem muon ve tren truc x
        private int YPTS;
        private double** z = null;
        private double[] clevel = null;
        private double xmin, xmax, ymin, ymax, zmin, zmax;
        private double[] x = null;
        private double[] y = null;

        static void cmap1_init()
        {
            double[] i = new double[2]; double[] h = new double[2];
            double[] l = new double[2]; double[] s = new double[2];

            i[0] = 0.0;         // left boundary
            i[1] = 1.0;         // right boundary

            h[0] = 240;         // blue -> green -> yellow ->
            h[1] = 0;           // -> red

            l[0] = 0.6;
            l[1] = 0.6;

            s[0] = 0.8;
            s[1] = 0.8;

            c_plscmap1n(256);
            c_plscmap1l(0, 2, i, h, l, s, null);
        }

        public void funtion3D(double xMin, double xMax, double yMin, double yMax, ListView list, String funtion3D)
        {
            xmin = xMin;
            xmax = xMax;
            ymin = yMin;
            ymax = yMax;

            XPTS = (int)((xMax - xMin) * 10);          // so diem muon ve tren truc x
            YPTS = (int)((yMax - yMin) * 10);           //

            int i, j;
            x = new double[XPTS];
            y = new double[YPTS];

            int nlevel = LEVELS;
            clevel = new double[LEVELS];
            double step;

            // Parse and process command line arguments

            // (void)plparseopts(&argc, argv, PL_PARSE_FULL);

            // Initialize plplot

            plAlloc2dGrid(ref z, XPTS, YPTS);
            double stepx = (xMax - xMin) / XPTS;
            double stepy = (yMax - yMin) / YPTS;
            for (i = 0; i < XPTS; i++)
            {
                x[i] = xMin + i * stepx;
            }

            for (i = 0; i < YPTS; i++)
            {
                y[i] = yMin + i * stepy;
            }

            for (i = 0; i < XPTS; i++)
            {
                for (j = 0; j < YPTS; j++)
                {
                    ExpressionContext context = new ExpressionContext();
                    // Allow the expression to use all static public methods of System.Math
                    context.Imports.AddType(typeof(Math));
                    // Define an int variable
                    context.Variables["x"] = x[i];
                    context.Variables["y"] = y[j];
                    IGenericExpression<double> eGeneric = context.CompileGeneric<double>(funtion3D);
                    // Evaluate the expressions
                    z[i][j] = (double)eGeneric.Evaluate();
                    if (j % 5 == 0) loadListView(x[i], y[j], z[i][j], list);
                }
            }

            plMinMax2dGrid(z, XPTS, YPTS, ref zmax, ref zmin);

            step = (zmax - zmin) / (nlevel + 1);

            for (i = 0; i < nlevel; i++)
                clevel[i] = zmin + step + step * i;

        }

        public void Paint3D(String funtion3D, double xoayOY, double xoayOZ)
        {
            c_plsdev("svg");
            c_plsfnam("demo3d");
            c_plinit();
            cmap1_init();
            c_pladv(0);
            c_plcol0(1);
            c_plvpor(0.0, 1.0, 0.0, 0.9);
            c_plwind(-1.0, 1.0, -1.0, 1.5);

            c_plw3d(1.0, 1.0, 1.2, xmin, xmax, ymin, ymax, zmin, (int)(zmax - 0.1) + 1, xoayOY, xoayOZ);
            c_plbox3("bnstu", "x axis", 0.0, 0,
                        "bnstu", "y axis", 0.0, 0,
                        "bcdmnstuv", "z axis", 0.0, 4);
            c_plcol0(2);

            c_plmeshc(x, y, z, XPTS, YPTS, DRAW_LINEXY | MAG_COLOR | BASE_CONT, clevel, 10);

            c_plcol0(3);
            c_plmtex("t", 1, 0.5, 0.5, "#frDo thi: z=" + funtion3D);

            //plFree2dGrid(z, XPTS, YPTS);

            c_plend();
        }

        private void loadListView(double x, double y, double z, ListView list)
        {
            String[] row = { x.ToString(), y.ToString(), z.ToString() };
            ListViewItem item = new ListViewItem(row);
            list.Items.Add(item);
        }
    }
}

