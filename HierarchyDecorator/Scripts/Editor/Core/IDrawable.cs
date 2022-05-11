namespace HierarchyDecorator
{
    public interface IDrawable
    {
        void OnDraw();
    }

    public interface IDrawable<T> : IDrawable
    {
        T Target { get; }
    }
}
