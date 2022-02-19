﻿using gESilk.engine.components;
using gESilk.engine.render.assets;
using gESilk.engine.render.materialSystem.settings;
using gESilk.engine.window;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using static gESilk.engine.Globals;
using static gESilk.Program;

namespace gESilk.engine.render.materialSystem;

public class Material
{
    private readonly ShaderProgram _program;
    private readonly List<ShaderSetting> _settings = new();
    private readonly DepthFunction _function;
    private CullFaceMode _cullFaceMode;
    private int _model, _view, _projection, _viewPos, _lightProj, _lightView;



    public Material(ShaderProgram program, DepthFunction function = DepthFunction.Less, CullFaceMode cullFaceMode = CullFaceMode.Back)
    {
        _program = program;
        _function = function;
        _cullFaceMode = cullFaceMode;
        
        _model = _program.GetUniform("model");
        _view = _program.GetUniform("view");
        _projection = _program.GetUniform("projection");
        _viewPos = _program.GetUniform("viewPos");
        _lightProj =  _program.GetUniform("lightProjection");
        _lightView = _program.GetUniform("lightView");
    }
    
    public void SetCullMode(CullFaceMode mode)
    {
        _cullFaceMode = mode;
    }

    public DepthFunction GetDepthFunction()
    {
        return _function;
    }


    public void Use(Matrix4 model, bool clearTranslation, DepthFunction? function = null)
    {
        GL.DepthFunc(function ?? _function);
        _program.Use();
        GL.CullFace(_cullFaceMode);
        _program.SetUniform(_model, model);
        EngineState state = MainWindow.State();
        if (state == EngineState.RenderShadowState)
        {
            _program.SetUniform(_view, ShadowView);
            _program.SetUniform(_projection , ShadowProjetion);
        }
        else
        {
            _program.SetUniform(_view,clearTranslation
                ? CameraSystem.CurrentCamera.View.ClearTranslation()
                : CameraSystem.CurrentCamera.View );
            _program.SetUniform(_projection, CameraSystem.CurrentCamera.Projection);
        }
        _program.SetUniform(_viewPos, CameraSystem.CurrentCamera.Entity.GetComponent<Transform>().Location);
        _program.SetUniform(_lightProj, ShadowProjetion);
        _program.SetUniform(_lightView, ShadowView);
        foreach (ShaderSetting setting in _settings)
        {
            setting.Use(_program);
        }
    }

    public void Cleanup()
    {
        foreach (ShaderSetting setting in _settings)
        {
            setting.Cleanup(_program);
        }
    }

    public void AddSetting(ShaderSetting setting)
    {
        _settings.Add(setting);
    }

    public void EditSetting(int index, ShaderSetting setting)
    {
        _settings[index] = setting;
    }
}