```
class T
{
  public int I{get;set;}
}

public static void Main()
{
  var t = new T
  {
    I = 10,
  };
  var x = t.I;
}
1. Where is `t` is allocated? heap --> reference types are allocated on the heap
2. Where is `t.I` is allocated? heap --> properties of reference types are allocated on the heap
3. do `x` and `t.I` are the same instance? no - value types are immutable
4. where is `x` allocated? stack - value types are allocated on the stack
