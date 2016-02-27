namespace CG.Web.MegaApiClient
{
  using System.IO;
  using System;

  internal class ProgressionStream : Stream
  {
    private readonly Stream baseStream;
    private readonly IProgress<double> progress;

    private long chunkSize;

    public ProgressionStream(Stream baseStream, IProgress<double> progress)
    {
      this.baseStream = baseStream;
      this.progress = progress;
    }

    public override int Read(byte[] array, int offset, int count)
    {
      int bytesRead = this.baseStream.Read(array, offset, count);
      this.ReportProgress(bytesRead);

      return bytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.baseStream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      this.progress.Report(100);
    }

    #region Forwards

    public override void Flush()
    {
      this.baseStream.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return this.baseStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      this.baseStream.SetLength(value);
    }

    public override bool CanRead => this.baseStream.CanRead;

    public override bool CanSeek => this.baseStream.CanSeek;

    public override bool CanWrite => this.baseStream.CanWrite;

    public override long Length => this.baseStream.Length;

    public override long Position
    {
      get { return this.baseStream.Position; }
      set { this.baseStream.Position = value; }
    }

    #endregion

    private void ReportProgress(int count)
    {
      this.chunkSize += count;
      if (this.chunkSize >= MegaApiClient.ReportProgressChunkSize)
      {
        this.chunkSize = 0;
        this.progress.Report(this.Position / (double)this.Length * 100);
      }
    }
  }
}