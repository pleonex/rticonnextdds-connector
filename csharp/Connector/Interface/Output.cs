﻿// ﻿   (c) 2005-2017 Copyright, Real-Time Innovations, All rights reserved.
//
// RTI grants Licensee a license to use, modify, compile, and create
// derivative works of the Software.  Licensee has the right to distribute
// object form only for use with RTI products. The Software is provided
// "as is", with no warranty of any type, including any warranty for fitness
// for any purpose. RTI is under no obligation to maintain or support the
// Software.  RTI shall not be liable for any incidental or consequential
// damages arising out of the use or inability to use the software.
namespace RTI.Connext.Connector.Interface
{
    using System;
    using System.Runtime.InteropServices;

    sealed class Output : IDisposable
    {
        readonly OutputPtr handle;

        public Output(Connector connector, string entityName)
        {
            Connector = connector;
            EntityName = entityName;
            handle = new OutputPtr(connector, entityName);
            if (handle.IsInvalid) {
                throw new COMException("Error getting output");
            }
        }

        ~Output()
        {
            Dispose(false);
        }

        public Connector Connector {
            get;
            private set;
        }

        public string EntityName {
            get;
            private set;
        }

        public void Clear()
        {
            if (Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            NativeMethods.RTIDDSConnector_clear(Connector.Handle, EntityName);
        }

        public void Write()
        {
            if (Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            NativeMethods.RTIDDSConnector_write(Connector.Handle, EntityName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool freeManagedResources)
        {
            if (freeManagedResources && !handle.IsInvalid) {
                handle.Dispose();
            }
        }

        static class NativeMethods
        {
            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern IntPtr RTIDDSConnector_getWriter(
                Connector.ConnectorPtr connectorHandle,
                string entityName);

            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern void RTIDDSConnector_clear(
                Connector.ConnectorPtr connectorHandle,
                string entityName);

            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern void RTIDDSConnector_write(
                Connector.ConnectorPtr connectorHandle,
                string entityName);
        }

        sealed class OutputPtr : SafeHandle
        {
            public OutputPtr(Connector connector, string entityName)
                : base(IntPtr.Zero, true)
            {
                handle = NativeMethods.RTIDDSConnector_getWriter(
                    connector.Handle,
                    entityName);
            }

            public override bool IsInvalid => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                return true;
            }
        }
    }
}