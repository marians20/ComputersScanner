using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Renci.SshNet.Common;

namespace Renci.SshNet.Sftp
{
    /// <summary>
    /// Contains SFTP file attributes.
    /// </summary>
    public class SftpFileAttributes
    {
        #region Bitmask constats

        private const UInt32 SIfmt = 0xF000; //  bitmask for the file type bitfields

        private const UInt32 SIfsock = 0xC000; //	socket

        private const UInt32 SIflnk = 0xA000; //	symbolic link

        private const UInt32 SIfreg = 0x8000; //	regular file

        private const UInt32 SIfblk = 0x6000; //	block device

        private const UInt32 SIfdir = 0x4000; //	directory

        private const UInt32 SIfchr = 0x2000; //	character device

        private const UInt32 SIfifo = 0x1000; //	FIFO

        private const UInt32 SIsuid = 0x0800; //	set UID bit

        private const UInt32 SIsgid = 0x0400; //	set-group-ID bit (see below)

        private const UInt32 SIsvtx = 0x0200; //	sticky bit (see below)

        private const UInt32 SIrusr = 0x0100; //	owner has read permission

        private const UInt32 SIwusr = 0x0080; //	owner has write permission

        private const UInt32 SIxusr = 0x0040; //	owner has execute permission

        private const UInt32 SIrgrp = 0x0020; //	group has read permission

        private const UInt32 SIwgrp = 0x0010; //	group has write permission

        private const UInt32 SIxgrp = 0x0008; //	group has execute permission

        private const UInt32 SIroth = 0x0004; //	others have read permission

        private const UInt32 SIwoth = 0x0002; //	others have write permission

        private const UInt32 SIxoth = 0x0001; //	others have execute permission

        #endregion

        private bool _isBitFiledsBitSet;
        private bool _isUidBitSet;
        private bool _isGroupIdBitSet;
        private bool _isStickyBitSet;

        private readonly DateTime _originalLastAccessTime;
        private readonly DateTime _originalLastWriteTime;
        private readonly long _originalSize;
        private readonly int _originalUserId;
        private readonly int _originalGroupId;
        private readonly uint _originalPermissions;
        private readonly IDictionary<string, string> _originalExtensions;

        internal bool IsLastAccessTimeChanged
        {
            get { return _originalLastAccessTime != LastAccessTime; }
        }

        internal bool IsLastWriteTimeChanged
        {
            get { return _originalLastWriteTime != LastWriteTime; }
        }

        internal bool IsSizeChanged
        {
            get { return _originalSize != Size; }
        }

        internal bool IsUserIdChanged
        {
            get { return _originalUserId != UserId; }
        }

        internal bool IsGroupIdChanged
        {
            get { return _originalGroupId != GroupId; }
        }

        internal bool IsPermissionsChanged
        {
            get { return _originalPermissions != Permissions; }
        }

        internal bool IsExtensionsChanged
        {
            get { return _originalExtensions != null && Extensions != null && !_originalExtensions.SequenceEqual(Extensions); }
        }

        /// <summary>
        /// Gets or sets the time the current file or directory was last accessed.
        /// </summary>
        /// <value>
        /// The time that the current file or directory was last accessed.
        /// </value>
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// Gets or sets the time when the current file or directory was last written to.
        /// </summary>
        /// <value>
        /// The time the current file was last written.
        /// </value>
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Gets or sets the size, in bytes, of the current file.
        /// </summary>
        /// <value>
        /// The size of the current file in bytes.
        /// </value>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets file user id.
        /// </summary>
        /// <value>
        /// File user id.
        /// </value>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets file group id.
        /// </summary>
        /// <value>
        /// File group id.
        /// </value>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets a value indicating whether file represents a socket.
        /// </summary>
        /// <value>
        ///   <c>true</c> if file represents a socket; otherwise, <c>false</c>.
        /// </value>
        public bool IsSocket { get; private set; }

        /// <summary>
        /// Gets a value indicating whether file represents a symbolic link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if file represents a symbolic link; otherwise, <c>false</c>.
        /// </value>
        public bool IsSymbolicLink { get; private set; }

        /// <summary>
        /// Gets a value indicating whether file represents a regular file.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if file represents a regular file; otherwise, <c>false</c>.
        /// </value>
        public bool IsRegularFile { get; private set; }

        /// <summary>
        /// Gets a value indicating whether file represents a block device.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if file represents a block device; otherwise, <c>false</c>.
        /// </value>
        public bool IsBlockDevice { get; private set; }

        /// <summary>
        /// Gets a value indicating whether file represents a directory.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if file represents a directory; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirectory { get; private set; }

        /// <summary>
        /// Gets a value indicating whether file represents a character device.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if file represents a character device; otherwise, <c>false</c>.
        /// </value>
        public bool IsCharacterDevice { get; private set; }

        /// <summary>
        /// Gets a value indicating whether file represents a named pipe.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if file represents a named pipe; otherwise, <c>false</c>.
        /// </value>
        public bool IsNamedPipe { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the owner can read from this file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if owner can read from this file; otherwise, <c>false</c>.
        /// </value>
        public bool OwnerCanRead { get; set; }

        /// <summary>
        /// Gets a value indicating whether the owner can write into this file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if owner can write into this file; otherwise, <c>false</c>.
        /// </value>
        public bool OwnerCanWrite { get; set; }

        /// <summary>
        /// Gets a value indicating whether the owner can execute this file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if owner can execute this file; otherwise, <c>false</c>.
        /// </value>
        public bool OwnerCanExecute { get; set; }

        /// <summary>
        /// Gets a value indicating whether the group members can read from this file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if group members can read from this file; otherwise, <c>false</c>.
        /// </value>
        public bool GroupCanRead { get; set; }

        /// <summary>
        /// Gets a value indicating whether the group members can write into this file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if group members can write into this file; otherwise, <c>false</c>.
        /// </value>
        public bool GroupCanWrite { get; set; }

        /// <summary>
        /// Gets a value indicating whether the group members can execute this file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if group members can execute this file; otherwise, <c>false</c>.
        /// </value>
        public bool GroupCanExecute { get; set; }

        /// <summary>
        /// Gets a value indicating whether the others can read from this file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if others can read from this file; otherwise, <c>false</c>.
        /// </value>
        public bool OthersCanRead { get; set; }

        /// <summary>
        /// Gets a value indicating whether the others can write into this file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if others can write into this file; otherwise, <c>false</c>.
        /// </value>
        public bool OthersCanWrite { get; set; }

        /// <summary>
        /// Gets a value indicating whether the others can execute this file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if others can execute this file; otherwise, <c>false</c>.
        /// </value>
        public bool OthersCanExecute { get; set; }

        /// <summary>
        /// Gets or sets the extensions.
        /// </summary>
        /// <value>
        /// The extensions.
        /// </value>
        public IDictionary<string, string> Extensions { get; private set; }

        internal uint Permissions
        {
            get
            {
                uint permission = 0;

                if (_isBitFiledsBitSet)
                    permission = permission | SIfmt;

                if (IsSocket)
                    permission = permission | SIfsock;

                if (IsSymbolicLink)
                    permission = permission | SIflnk;

                if (IsRegularFile)
                    permission = permission | SIfreg;

                if (IsBlockDevice)
                    permission = permission | SIfblk;

                if (IsDirectory)
                    permission = permission | SIfdir;

                if (IsCharacterDevice)
                    permission = permission | SIfchr;

                if (IsNamedPipe)
                    permission = permission | SIfifo;

                if (_isUidBitSet)
                    permission = permission | SIsuid;

                if (_isGroupIdBitSet)
                    permission = permission | SIsgid;

                if (_isStickyBitSet)
                    permission = permission | SIsvtx;

                if (OwnerCanRead)
                    permission = permission | SIrusr;

                if (OwnerCanWrite)
                    permission = permission | SIwusr;

                if (OwnerCanExecute)
                    permission = permission | SIxusr;

                if (GroupCanRead)
                    permission = permission | SIrgrp;

                if (GroupCanWrite)
                    permission = permission | SIwgrp;

                if (GroupCanExecute)
                    permission = permission | SIxgrp;

                if (OthersCanRead)
                    permission = permission | SIroth;

                if (OthersCanWrite)
                    permission = permission | SIwoth;

                if (OthersCanExecute)
                    permission = permission | SIxoth;

                return permission;
            }
            private set
            {
                _isBitFiledsBitSet = ((value & SIfmt) == SIfmt);

                IsSocket = ((value & SIfsock) == SIfsock);

                IsSymbolicLink = ((value & SIflnk) == SIflnk);

                IsRegularFile = ((value & SIfreg) == SIfreg);

                IsBlockDevice = ((value & SIfblk) == SIfblk);

                IsDirectory = ((value & SIfdir) == SIfdir);

                IsCharacterDevice = ((value & SIfchr) == SIfchr);

                IsNamedPipe = ((value & SIfifo) == SIfifo);

                _isUidBitSet = ((value & SIsuid) == SIsuid);

                _isGroupIdBitSet = ((value & SIsgid) == SIsgid);

                _isStickyBitSet = ((value & SIsvtx) == SIsvtx);

                OwnerCanRead = ((value & SIrusr) == SIrusr);

                OwnerCanWrite = ((value & SIwusr) == SIwusr);

                OwnerCanExecute = ((value & SIxusr) == SIxusr);

                GroupCanRead = ((value & SIrgrp) == SIrgrp);

                GroupCanWrite = ((value & SIwgrp) == SIwgrp);

                GroupCanExecute = ((value & SIxgrp) == SIxgrp);

                OthersCanRead = ((value & SIroth) == SIroth);

                OthersCanWrite = ((value & SIwoth) == SIwoth);

                OthersCanExecute = ((value & SIxoth) == SIxoth);
            }
        }

        private SftpFileAttributes()
        {
        }

        internal SftpFileAttributes(DateTime lastAccessTime, DateTime lastWriteTime, long size, int userId, int groupId, uint permissions, IDictionary<string, string> extensions)
        {
            LastAccessTime = _originalLastAccessTime = lastAccessTime;
            LastWriteTime = _originalLastWriteTime = lastWriteTime;
            Size = _originalSize = size;
            UserId = _originalUserId = userId;
            GroupId = _originalGroupId = groupId;
            Permissions = _originalPermissions = permissions;
            Extensions = _originalExtensions = extensions;
        }

        /// <summary>
        /// Sets the permissions.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public void SetPermissions(short mode)
        {
            if (mode < 0 || mode > 999)
            {
                throw new ArgumentOutOfRangeException("mode");
            }

            var modeBytes = mode.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0').ToCharArray();

            var permission = (modeBytes[0] & 0x0F) * 8 * 8 + (modeBytes[1] & 0x0F) * 8 + (modeBytes[2] & 0x0F);

            OwnerCanRead = (permission & SIrusr) == SIrusr;
            OwnerCanWrite = (permission & SIwusr) == SIwusr;
            OwnerCanExecute = (permission & SIxusr) == SIxusr;

            GroupCanRead = (permission & SIrgrp) == SIrgrp;
            GroupCanWrite = (permission & SIwgrp) == SIwgrp;
            GroupCanExecute = (permission & SIxgrp) == SIxgrp;

            OthersCanRead = (permission & SIroth) == SIroth;
            OthersCanWrite = (permission & SIwoth) == SIwoth;
            OthersCanExecute = (permission & SIxoth) == SIxoth;
        }

        /// <summary>
        /// Returns a byte array representing the current <see cref="SftpFileAttributes"/>.
        /// </summary>
        /// <returns>
        /// A byte array representing the current <see cref="SftpFileAttributes"/>.
        /// </returns>
        public byte[] GetBytes()
        {
            var stream = new SshDataStream(4);

            uint flag = 0;

            if (IsSizeChanged && IsRegularFile)
            {
                flag |= 0x00000001;
            }

            if (IsUserIdChanged || IsGroupIdChanged)
            {
                flag |= 0x00000002;
            }

            if (IsPermissionsChanged)
            {
                flag |= 0x00000004;
            }

            if (IsLastAccessTimeChanged || IsLastWriteTimeChanged)
            {
                flag |= 0x00000008;
            }

            if (IsExtensionsChanged)
            {
                flag |= 0x80000000;
            }

            stream.Write(flag);

            if (IsSizeChanged && IsRegularFile)
            {
                stream.Write((ulong) Size);
            }

            if (IsUserIdChanged || IsGroupIdChanged)
            {
                stream.Write((uint) UserId);
                stream.Write((uint) GroupId);
            }

            if (IsPermissionsChanged)
            {
                stream.Write(Permissions);
            }

            if (IsLastAccessTimeChanged || IsLastWriteTimeChanged)
            {
                var time = (uint)(LastAccessTime.ToFileTime() / 10000000 - 11644473600);
                stream.Write(time);
                time = (uint)(LastWriteTime.ToFileTime() / 10000000 - 11644473600);
                stream.Write(time);
            }

            if (IsExtensionsChanged)
            {
                foreach (var item in Extensions)
                {
                    // TODO: we write as ASCII but read as UTF8 !!!

                    stream.Write(item.Key, SshData.Ascii);
                    stream.Write(item.Value, SshData.Ascii);
                }
            }

            return stream.ToArray();
        }

        internal static readonly SftpFileAttributes Empty = new SftpFileAttributes();

        internal static SftpFileAttributes FromBytes(SshDataStream stream)
        {
            var flag = stream.ReadUInt32();

            long size = -1;
            var userId = -1;
            var groupId = -1;
            uint permissions = 0;
            var accessTime = DateTime.MinValue;
            var modifyTime = DateTime.MinValue;
            IDictionary<string, string> extensions = null;

            if ((flag & 0x00000001) == 0x00000001)   //  SSH_FILEXFER_ATTR_SIZE
            {
                size = (long) stream.ReadUInt64();
            }

            if ((flag & 0x00000002) == 0x00000002)   //  SSH_FILEXFER_ATTR_UIDGID
            {
                userId = (int) stream.ReadUInt32();

                groupId = (int) stream.ReadUInt32();
            }

            if ((flag & 0x00000004) == 0x00000004)   //  SSH_FILEXFER_ATTR_PERMISSIONS
            {
                permissions = stream.ReadUInt32();
            }

            if ((flag & 0x00000008) == 0x00000008)   //  SSH_FILEXFER_ATTR_ACMODTIME
            {
                var time = stream.ReadUInt32();
                accessTime = DateTime.FromFileTime((time + 11644473600) * 10000000);
                time = stream.ReadUInt32();
                modifyTime = DateTime.FromFileTime((time + 11644473600) * 10000000);
            }

            if ((flag & 0x80000000) == 0x80000000)   //  SSH_FILEXFER_ATTR_EXTENDED
            {
                var extendedCount = (int) stream.ReadUInt32();
                extensions = new Dictionary<string, string>(extendedCount);
                for (var i = 0; i < extendedCount; i++)
                {
                    var extensionName = stream.ReadString(SshData.Utf8);
                    var extensionData = stream.ReadString(SshData.Utf8);
                    extensions.Add(extensionName, extensionData);
                }
            }

            return new SftpFileAttributes(accessTime, modifyTime, size, userId, groupId, permissions, extensions);
        }

        internal static SftpFileAttributes FromBytes(byte[] buffer)
        {
            using (var stream = new SshDataStream(buffer))
            {
                return FromBytes(stream);
            }
        }
    }
}
