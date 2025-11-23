using System;
using LibRusted.Core.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LibRusted.World2D;

public class Camera2DComponent(Viewport viewport) : IComponent
{
    private Vector2 _position = Vector2.Zero;
    private float _rotation;
    private float _zoom = 1f;
    private readonly Viewport _viewport = viewport;
    private Rectangle? _limits;
    
    private Matrix _viewMatrix;
    private Matrix _inverseViewMatrix;
    private Rectangle _bounds;
    private bool _isDirty = true;

    public Vector2 Position
    {
        get => _position;
        set
        {
            if (_position == value) return;
            _position = value;
            _isDirty = true;
        }
    }

    public float Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation.Equals(value)) return;
            _rotation = MathHelper.WrapAngle(value);
            _isDirty = true;
        }
    }

    public float Zoom
    {
        get => _zoom;
        set
        {
            var newZoom = MathHelper.Clamp(value, 0.1f, 5f);
            if (!(Math.Abs(_zoom - newZoom) > float.Epsilon)) return;
            _zoom = newZoom;
            _isDirty = true;
        }
    }

    public Rectangle? Limits
    {
        get => _limits;
        set
        {
            if (_limits == value) return;
            _limits = value;
            _isDirty = true;
        }
    }

    public Matrix ViewMatrix
    {
        get
        {
            if (_isDirty)
                UpdateMatrices();
            return _viewMatrix;
        }
    }

    public Matrix InverseViewMatrix
    {
        get
        {
            if (_isDirty)
                UpdateMatrices();
            return _inverseViewMatrix;
        }
    }

    public Rectangle Bounds
    {
        get
        {
            if (_isDirty)
                UpdateMatrices();
            return _bounds;
        }
    }

    private void UpdateMatrices()
    {
        if (!_isDirty) return;
        if (_limits.HasValue)
        {
            _position = ClampToLimits(_position);
        }
        _viewMatrix = Matrix.CreateTranslation(-_position.X, -_position.Y, 0) *
                      Matrix.CreateRotationZ(_rotation) *
                      Matrix.CreateScale(_zoom, _zoom, 1) *
                      Matrix.CreateTranslation(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0);
        
        _inverseViewMatrix = Matrix.Invert(_viewMatrix);
        UpdateBounds();
        _isDirty = false;
    }

    private void UpdateBounds()
    {
        var topLeft = Vector2.Transform(Vector2.Zero, _inverseViewMatrix);
        var bottomRight = Vector2.Transform(new Vector2(_viewport.Width, _viewport.Height), _inverseViewMatrix);
        
        _bounds = new Rectangle(
            (int)topLeft.X, (int)topLeft.Y,
            (int)(bottomRight.X - topLeft.X),
            (int)(bottomRight.Y - topLeft.Y)
        );
    }

    private Vector2 ClampToLimits(Vector2 position)
    {
        if (!_limits.HasValue) return position;

        var bounds = Bounds;
        var limits = _limits.Value;

        if (bounds.Width <= limits.Width)
        {
            position.X = MathHelper.Clamp(position.X,
                limits.Left + bounds.Width * 0.5f,
                limits.Right - bounds.Width * 0.5f);
        }

        if (bounds.Height <= limits.Height)
        {
            position.Y = MathHelper.Clamp(position.Y,
                limits.Top + bounds.Height * 0.5f,
                limits.Bottom - bounds.Height * 0.5f);
        }

        return position;
    }

   
    public void SetTransform(Vector2? position = null, float? rotation = null, float? zoom = null)
    {
        var changed = false;

        if (position.HasValue && _position != position.Value)
        {
            _position = position.Value;
            changed = true;
        }

        if (rotation.HasValue && Math.Abs(_rotation - rotation.Value) > float.Epsilon)
        {
            _rotation = MathHelper.WrapAngle(rotation.Value);
            changed = true;
        }

        if (zoom.HasValue && Math.Abs(_zoom - zoom.Value) > float.Epsilon)
        {
            _zoom = MathHelper.Clamp(zoom.Value, 0.1f, 5f);
            changed = true;
        }

        if (changed)
            _isDirty = true;
    }

    public void Move(Vector2 direction)
    {
        if (direction != Vector2.Zero)
        {
            _position += direction;
            _isDirty = true;
        }
    }

    public void LookAt(Vector2 worldPosition)
    {
        if (Position != worldPosition)
        {
            Position = worldPosition;
        }
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, InverseViewMatrix);
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, ViewMatrix);
    }

    public bool IsInView(Vector2 worldPosition)
    {
        return Bounds.Contains(worldPosition);
    }

    public bool IsInView(Rectangle bounds)
    {
        return Bounds.Intersects(bounds);
    }
}