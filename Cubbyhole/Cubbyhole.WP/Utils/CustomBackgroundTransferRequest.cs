using Microsoft.Phone.BackgroundTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubbyhole.WP.Utils
{
    public class CustomBackgroundTransferRequest
    {
        private BackgroundTransferRequest _request;
        public BackgroundTransferRequest Request
        {
            get
            {
                return _request;
            }
            set
            {
                _request = value;
                Init();
            }
        }
        public string CompletionPercent { get; private set; }
        public long RealTotalSize { get; private set; }
        public string TransferType { get; private set; }
        public string FileName { get; private set; }

        public CustomBackgroundTransferRequest()
        {
        }

        public CustomBackgroundTransferRequest(BackgroundTransferRequest request)
        {
            this.Request = request;
            Init();
        }

        private void Init()
        {
            GetRealTotalSize();
            CalculateCompletion();
            SetTransferType();
            GetFileName();
        }

        private void CalculateCompletion()
        {
            var percent = 0.0;
            if(Request.Method.Equals("GET"))
            {
                if (RealTotalSize != 0)
                {
                    percent = (Request.BytesReceived * 100) / RealTotalSize;
                }
            }
            else
            {
                if (Request.TotalBytesToSend != 0)
                {
                    percent = (Request.BytesSent * 100) / Request.TotalBytesToSend;
                }
            }
            CompletionPercent = percent + "%";
        }

        // Request.TotalBytesToReceive value is -1 ...
        // So we have to trick here
        private void GetRealTotalSize()
        {
            long realSize;
            long.TryParse(Request.Tag.Split('~').First(), out realSize);
            RealTotalSize = realSize;
        }

        private void SetTransferType()
        {
            if (Request.Method.Equals("POST"))
            {
                TransferType = "Upload";
            }
            else
            {
                TransferType = "Download";
            }
        }

        private void GetFileName()
        {
            FileName = Request.Tag.Split('~').Last();
        }
    }
}
