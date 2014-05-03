public struct int3
{
    public int x, y, z;
    public int3(int X, int Y, int Z)
    {
        x = X;
        y = Y;
        z = Z;
    }
    public bool Equals(int3 other)
    {
        return (x == other.x && y == other.y && z == other.z);
        //return (other != null &&
        //        other.CustId == this.CustId &&
        //        other.CustName == this.CustName);
    }
   public static bool operator !=(int3 c1, int3 c2) 
   {
       return !c1.Equals(c2);
   }
   public static bool operator ==(int3 c1, int3 c2) 
   {
       return c1.Equals(c2);
   }
}
