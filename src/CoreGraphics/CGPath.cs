// 
// CGPath.cs: Implements the managed CGPath
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Drawing;
using System.Runtime.InteropServices;

using MonoMac.ObjCRuntime;
using MonoMac.Foundation;

namespace MonoMac.CoreGraphics {
	public enum CGPathElementType {
		MoveToPoint,
		AddLineToPoint,
		AddQuadCurveToPoint,
		AddCurveToPoint,
		CloseSubpath
	}

	public struct CGPathElement {
		public CGPathElementType Type;

		public CGPathElement (int t)
		{
			Type = (CGPathElementType) t;
			Point1 = Point2 = Point3 = new PointF (0,0);
		}
		
		// Set for MoveToPoint, AddLineToPoint, AddQuadCurveToPoint, AddCurveToPoint
		public PointF Point1;

		// Set for AddQuadCurveToPoint, AddCurveToPoint
		public PointF Point2;

		// Set for AddCurveToPoint
		public PointF Point3;
	}
	
	public class CGPath : INativeObject, IDisposable {
		internal IntPtr handle;

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGPathCreateMutable();
		public CGPath ()
		{
			handle = CGPathCreateMutable ();
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGPathCreateMutableCopy(IntPtr path);
		public CGPath (CGPath basePath)
		{
			if (basePath == null)
				throw new ArgumentNullException ("basePath");
			handle = CGPathCreateMutableCopy (basePath.handle);
		}

		//
		// For use by marshallrs
		//
		internal CGPath (IntPtr handle)
		{
			CGPathRetain (handle);
			this.handle = handle;
		}

		// Indicates that we own it `owns'
		[Preserve (Conditional=true)]
		internal CGPath (IntPtr handle, bool owns)
		{
			if (!owns)
				CGPathRetain (handle);
			
			this.handle = handle;
		}
		
		~CGPath ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
	
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathRelease (IntPtr handle);
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathRetain (IntPtr handle);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGPathRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPathEqualToPath(IntPtr path1, IntPtr path2);

		public static bool operator == (CGPath path1, CGPath path2)
		{
			return Object.Equals (path1, path2);
		}

		public static bool operator != (CGPath path1, CGPath path2)
		{
			return !Object.Equals (path1, path2);
		}

		public override int GetHashCode ()
		{
			return handle.GetHashCode ();
		}

		public override bool Equals (object o)
		{
			CGPath other = o as CGPath;
			if (other == null)
				return false;

			return CGPathEqualToPath (this.handle, other.handle);
		}
       
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathMoveToPoint(IntPtr path, ref CGAffineTransform m, float x, float y);
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathMoveToPoint(IntPtr path, IntPtr zero, float x, float y);
		public void MoveToPoint (float x, float y)
		{
			CGPathMoveToPoint (handle, IntPtr.Zero, x, y);
		}

		public void MoveToPoint (CGAffineTransform transform, float x, float y)
		{
			CGPathMoveToPoint (handle, ref transform, x, y);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddLineToPoint(IntPtr path, ref CGAffineTransform m, float x, float y);
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddLineToPoint(IntPtr path, IntPtr m, float x, float y);
		public void CGPathAddLineToPoint (float x, float y)
		{
			CGPathAddLineToPoint (handle, IntPtr.Zero, x, y);
		}

		public void CGPathAddLineToPoint (CGAffineTransform transform, float x, float y)
		{
			CGPathAddLineToPoint (handle, ref transform, x, y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddQuadCurveToPoint(IntPtr path, ref CGAffineTransform m, float cpx, float cpy, float x, float y);
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddQuadCurveToPoint(IntPtr path, IntPtr zero, float cpx, float cpy, float x, float y);
		public void AddQuadCurveToPoint (float cpx, float cpy, float x, float y)
		{
			CGPathAddQuadCurveToPoint (handle, IntPtr.Zero, cpx, cpy, x, y);
		}

		public void AddQuadCurveToPoint (CGAffineTransform transform, float cpx, float cpy, float x, float y)
		{
			CGPathAddQuadCurveToPoint (handle, ref transform, cpx, cpy, x, y);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddCurveToPoint(IntPtr path, ref CGAffineTransform m, float cp1x, float cp1y, float cp2x, float cp2y, float x, float y);
		public void AddCurveToPoint (CGAffineTransform transform, float cp1x, float cp1y, float cp2x, float cp2y, float x, float y)
		{
			CGPathAddCurveToPoint (handle, ref transform, cp1x, cp1y, cp2x, cp2y, x, y);
		}
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddCurveToPoint(IntPtr path, IntPtr zero, float cp1x, float cp1y, float cp2x, float cp2y, float x, float y);
		public void AddCurveToPoint (float cp1x, float cp1y, float cp2x, float cp2y, float x, float y)
		{
			CGPathAddCurveToPoint (handle, IntPtr.Zero, cp1x, cp1y, cp2x, cp2y, x, y);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathCloseSubpath(IntPtr path);
		public void CloseSubpath ()
		{
			CGPathCloseSubpath (handle);
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddRect(IntPtr path, ref CGAffineTransform m, RectangleF rect);
		public void AddRect (CGAffineTransform transform, RectangleF rect)
		{
			CGPathAddRect (handle, ref transform, rect);
		}
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddRect(IntPtr path, IntPtr zero, RectangleF rect);
		public void AddRect (RectangleF rect)
		{
			CGPathAddRect (handle, IntPtr.Zero, rect);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddRects(IntPtr path, ref CGAffineTransform m, RectangleF [] rects, int size_t_count);
		public void AddRects (CGAffineTransform m, RectangleF [] rects)
		{
			CGPathAddRects (handle, ref m, rects, rects.Length);
		}
		public void AddRects (CGAffineTransform m, RectangleF [] rects, int count)
		{
			if (count > rects.Length)
				throw new ArgumentException ("counts");
			CGPathAddRects (handle, ref m, rects, count);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddRects(IntPtr path, IntPtr Zero, RectangleF [] rects, int size_t_count);
		public void AddRects (RectangleF [] rects)
		{
			CGPathAddRects (handle, IntPtr.Zero, rects, rects.Length);
		}
		public void AddRects (RectangleF [] rects, int count)
		{
			if (count > rects.Length)
				throw new ArgumentException ("count");
			CGPathAddRects (handle, IntPtr.Zero, rects, count);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddLines(IntPtr path, ref CGAffineTransform m, PointF [] points, int size_t_count);
		public void AddLines (CGAffineTransform m, PointF [] points)
		{
			CGPathAddLines (handle, ref m, points, points.Length);
		}
		public void AddRects (CGAffineTransform m, PointF [] points, int count)
		{
			if (count > points.Length)
				throw new ArgumentException ("count");
			CGPathAddLines (handle, ref m, points, count);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddLines(IntPtr path, IntPtr zero, PointF [] points, int size_t_count);
		public void AddLines (PointF [] points)
		{
			CGPathAddLines (handle, IntPtr.Zero, points, points.Length);
		}
		public void AddRects (PointF [] points, int count)
		{
			if (count > points.Length)
				throw new ArgumentException ("count");
			CGPathAddLines (handle, IntPtr.Zero, points, count);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddEllipseInRect(IntPtr path, ref CGAffineTransform m, RectangleF rect);
		public void AddElipseInRect (CGAffineTransform m, RectangleF rect)
		{
			CGPathAddEllipseInRect (handle, ref m, rect);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddEllipseInRect(IntPtr path, IntPtr zero, RectangleF rect);
		public void AddElipseInRect (RectangleF rect)
		{
			CGPathAddEllipseInRect (handle, IntPtr.Zero, rect);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddArc(IntPtr path, ref CGAffineTransform m, float x, float y, float radius, float startAngle, float endAngle, bool clockwise);
		public void AddArc (CGAffineTransform m, float x, float y, float radius, float startAngle, float endAngle, bool clockwise)
		{
			CGPathAddArc (handle, ref m, x, y, radius, startAngle, endAngle, clockwise);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddArc(IntPtr path, IntPtr zero, float x, float y, float radius, float startAngle, float endAngle, bool clockwise);
		public void AddArc (float x, float y, float radius, float startAngle, float endAngle, bool clockwise)
		{
			CGPathAddArc (handle, IntPtr.Zero, x, y, radius, startAngle, endAngle, clockwise);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddArcToPoint(IntPtr path, ref CGAffineTransform m, float x1, float y1, float x2, float y2, float radius);
		public void AddArcToPoint (CGAffineTransform m, float x1, float y1, float x2, float y2, float radius)
		{
			CGPathAddArcToPoint (handle, ref m, x1, y1, x2, y2, radius);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddArcToPoint(IntPtr path, IntPtr zero, float x1, float y1, float x2, float y2, float radius);
		public void AddArcToPoint (float x1, float y1, float x2, float y2, float radius)
		{
			CGPathAddArcToPoint (handle, IntPtr.Zero, x1, y1, x2, y2, radius);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddPath(IntPtr path1, ref CGAffineTransform m, IntPtr path2);
		public void AddPath (CGAffineTransform t, CGPath path2)
		{
			if (path2 == null)
				throw new ArgumentNullException ("path2");
			CGPathAddPath (handle, ref t, path2.handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathAddPath(IntPtr path1, IntPtr zero, IntPtr path2);
		public void AddPath (CGPath path2)
		{
			if (path2 == null)
				throw new ArgumentNullException ("path2");
			CGPathAddPath (handle, IntPtr.Zero, path2.handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static int CGPathIsEmpty(IntPtr path);
		public bool IsEmpty {
			get {
				return CGPathIsEmpty (handle) != 0;
			}
		}
			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static int CGPathIsRect(IntPtr path, out RectangleF rect);
		public bool IsRect (out RectangleF rect)
		{
			return CGPathIsRect (handle, out rect) != 0;
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static PointF CGPathGetCurrentPoint(IntPtr path);
		public PointF CurrentPoint {
			get {
				return CGPathGetCurrentPoint (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static RectangleF CGPathGetBoundingBox(IntPtr path);
		public RectangleF BoundingBox {
			get {
				return CGPathGetBoundingBox (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static RectangleF CGPathGetPathBoundingBox(IntPtr path);
		public RectangleF PathBoundingBox {
			get {
				return CGPathGetPathBoundingBox (handle);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPathContainsPoint(IntPtr path, ref CGAffineTransform m, PointF point, bool eoFill);
		public bool ContainsPoint (CGAffineTransform m, PointF point, bool eoFill)
		{
			return CGPathContainsPoint (handle, ref m, point, eoFill);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPathContainsPoint(IntPtr path, IntPtr zero, PointF point, bool eoFill);
		public bool ContainsPoint (PointF point, bool eoFill)
		{
			return CGPathContainsPoint (handle, IntPtr.Zero, point, eoFill);
		}

		//typedef void (*CGPathApplierFunction)(void *info, const CGPathElement *element);
		public delegate void ApplierFunction (CGPathElement element);
		delegate void CGPathApplierFunction (IntPtr info, IntPtr CGPathElementPtr);
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPathApply(IntPtr path, IntPtr info, CGPathApplierFunction function);
		public void Apply (ApplierFunction func)
		{
			CGPathApply (handle, IntPtr.Zero, delegate (IntPtr info, IntPtr element_ptr){


				// Fetch the data, which is in this layout:
				// struct _CGPathElement {
				//     CGPathElementType type;
				//     PointFs *points;
				// }

				CGPathElement element = new CGPathElement (Marshal.ReadInt32 (element_ptr, 0));

				IntPtr ptr = Marshal.ReadIntPtr (element_ptr, 4);
				switch (element.Type){
				case CGPathElementType.CloseSubpath:
					break;
					
				case CGPathElementType.MoveToPoint:
				case CGPathElementType.AddLineToPoint:
					element.Point1 = (PointF) Marshal.PtrToStructure (ptr, typeof (PointF));
					break;

				case CGPathElementType.AddQuadCurveToPoint:
					element.Point1 = (PointF) Marshal.PtrToStructure (ptr, typeof (PointF));
					element.Point2 = (PointF) Marshal.PtrToStructure (((IntPtr) (((long)ptr) + IntPtr.Size)), typeof (PointF));
					break;
					
				case CGPathElementType.AddCurveToPoint:
					element.Point1 = (PointF) Marshal.PtrToStructure (ptr, typeof (PointF));
					element.Point2 = (PointF) Marshal.PtrToStructure (((IntPtr) (((long)ptr) + IntPtr.Size)), typeof (PointF));
					element.Point3 = (PointF) Marshal.PtrToStructure (((IntPtr) (((long)ptr) + (2*IntPtr.Size))), typeof (PointF));
					break;
				}

				func (element);
			});
		}

	}
}
