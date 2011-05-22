#include "stdafx.h"
#pragma once

using namespace System::Reflection;
using namespace System::Collections::Generic;

namespace CefSharp
{
    class binding_data : public CefBase
    {
    protected:
        gcroot<Object^> _obj;
    };

    class BindingData : public CefThreadSafeBase<binding_data>
    {
    public:
        BindingData(Object^ obj)
        {
            _obj = obj;
        }

        Object^ Get()
        {
            return _obj;
        }
    };

    class BindingHandler : public CefThreadSafeBase<CefV8Handler>
    {
        // Type Converter
        static bool IsNullableType(Type^ type);
        static int GetChangeTypeCost(Object^ value, Type^ conversionType);
        static Object^ ChangeType(Object^ value, Type^ conversionType);

        CefRefPtr<CefV8Value> ConvertToCef(Object^ obj, Type^ type);
        Object^ ConvertFromCef(CefRefPtr<CefV8Value> obj);
        virtual bool Execute(const CefString& name, CefRefPtr<CefV8Value> object, const CefV8ValueList& arguments, CefRefPtr<CefV8Value>& retval, CefString& exception);
    public:
        static void Bind(String^ name, Object^ obj, CefRefPtr<CefV8Value> window);
    };
}