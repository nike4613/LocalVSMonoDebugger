﻿using LocalMonoDebugger.Services;
using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Runtime.InteropServices;

namespace Mono.Debugging.VisualStudio
{
    [Guid(DebugEngineGuids.MonoProgramProviderString)]
    public class MonoPortSupplier : IDebugPortSupplier2
    {
        protected PortSupplier _portSupplier;
        public MonoPortSupplier()
        {
            _portSupplier = new PortSupplier();
        }

        public int GetPortSupplierName(out string pbstrName)
        {
            return ((IDebugPortSupplier2)_portSupplier).GetPortSupplierName(out pbstrName);
        }

        public int GetPortSupplierId(out Guid pguidPortSupplier)
        {
            var result = ((IDebugPortSupplier2)_portSupplier).GetPortSupplierId(out pguidPortSupplier);
            pguidPortSupplier = new Guid(DebugEngineGuids.MonoProgramProviderString);
            return result;
        }

        public int GetPort(ref Guid guidPort, out IDebugPort2 ppPort)
        {
            return ((IDebugPortSupplier2)_portSupplier).GetPort(ref guidPort, out ppPort);
        }

        public int EnumPorts(out IEnumDebugPorts2 ppEnum)
        {
            return ((IDebugPortSupplier2)_portSupplier).EnumPorts(out ppEnum);
        }

        public int CanAddPort()
        {
            return ((IDebugPortSupplier2)_portSupplier).CanAddPort();
        }

        public int AddPort(IDebugPortRequest2 pRequest, out IDebugPort2 ppPort)
        {
            return ((IDebugPortSupplier2)_portSupplier).AddPort(pRequest, out ppPort);
        }

        public int RemovePort(IDebugPort2 pPort)
        {
            return ((IDebugPortSupplier2)_portSupplier).RemovePort(pPort);
        }
    }
}
