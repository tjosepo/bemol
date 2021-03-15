namespace Bemol {
  public class Middleware {
    public readonly Handler Handler;

    public Middleware(Handler handler) => Handler = handler;
  }
}