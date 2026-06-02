using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace GSP_DatToExcel
{
	public class WemadeCrypt
	{
		#region BlowFish
		byte[] key;
		/// <summary>
		///   Block size in bytes.
		/// </summary>
		public const int BLOCK_SIZE = 8;

		/// <summary>
		///   Maximum (and recommended) key size in bytes.
		/// </summary>
		public const int MAX_KEY_LENGTH = 56;

		//////////////////////////////////////////////////////////////////////
		public static uint[] PBOX_INIT = new uint[PBOX_ENTRIES];
		public static uint[] SBOX_INIT_1 = new uint[SBOX_ENTRIES];
		public static uint[] SBOX_INIT_2 = new uint[SBOX_ENTRIES];
		public static uint[] SBOX_INIT_3 = new uint[SBOX_ENTRIES];
		public static uint[] SBOX_INIT_4 = new uint[SBOX_ENTRIES];

        protected byte[] m_block = new byte[BLOCK_SIZE];
		protected uint[] m_pbox = new uint[PBOX_ENTRIES];
		protected uint[] m_sbox1 = new uint[SBOX_ENTRIES];
		protected uint[] m_sbox2 = new uint[SBOX_ENTRIES];
		protected uint[] m_sbox3 = new uint[SBOX_ENTRIES];
		protected uint[] m_sbox4 = new uint[SBOX_ENTRIES];

		//////////////////////////////////////////////////////////////////////
		const int PBOX_ENTRIES = 18;
		const int SBOX_ENTRIES = 256;

		//////////////////////////////////////////////////////////////////////
		/// <summary>
		///   Resets the instance with a key material. Allows the switch of
		///   keys at runtime without any object allocation.
		/// </summary>
		/// <param name="key"> 
		///   Buffer with the (binary) key material.
		/// </param>
		/// <param name="nOfs"> 
		///   Position at which the key material starts in the buffer.
		/// </param>
		/// <param name="key"> 
		///   Size of the key material (up to MAX_KEY_LENGTH bytes).
		/// </param>
		public void Initialize(byte[] key, int nOfs, int nLen)
		{
			int nI, nJ, nKeyPos, nKeyEnd;
			uint unBuild, unHi, unLo;
			uint[] box;

			Array.Copy(PBOX_INIT, 0, m_pbox, 0, PBOX_INIT.Length);
			Array.Copy(SBOX_INIT_1, 0, m_sbox1, 0, SBOX_INIT_1.Length);
			Array.Copy(SBOX_INIT_2, 0, m_sbox2, 0, SBOX_INIT_2.Length);
			Array.Copy(SBOX_INIT_3, 0, m_sbox3, 0, SBOX_INIT_3.Length);
			Array.Copy(SBOX_INIT_4, 0, m_sbox4, 0, SBOX_INIT_4.Length);

			if (nOfs < 0)
				throw new ArgumentOutOfRangeException("nOfs");
			if (nLen < 0 || nLen > key.Length)
				throw new ArgumentOutOfRangeException("nLen");
			if (key.Length < nOfs + nLen)
				throw new ArgumentOutOfRangeException("nLen or nOfs");
			if (0 == nLen)
				nLen = key.Length - nOfs;

			nKeyPos = nOfs;
			nKeyEnd = nOfs + nLen;
			unBuild = 0;

			for (nI = 0; nI < PBOX_ENTRIES; nI++)
			{
				for (nJ = 0; nJ < 4; nJ++)
				{
					unBuild = (unBuild << 8) | key[nKeyPos];

					if (++nKeyPos == nKeyEnd)
					{
						nKeyPos = nOfs;
					}
				}
				m_pbox[nI] ^= unBuild;
			}

			unHi = unLo = 0;

			box = m_pbox;
			for (nI = 0; nI < PBOX_ENTRIES;)
			{
				EncryptBlock(unHi, unLo, out unHi, out unLo);
				box[nI++] = unHi;
				box[nI++] = unLo;
			}
			box = m_sbox1;
			for (nI = 0; nI < SBOX_ENTRIES;)
			{
				EncryptBlock(unHi, unLo, out unHi, out unLo);
				box[nI++] = unHi;
				box[nI++] = unLo;
			}
			box = m_sbox2;
			for (nI = 0; nI < SBOX_ENTRIES;)
			{
				EncryptBlock(unHi, unLo, out unHi, out unLo);
				box[nI++] = unHi;
				box[nI++] = unLo;
			}
			box = m_sbox3;
			for (nI = 0; nI < SBOX_ENTRIES;)
			{
				EncryptBlock(unHi, unLo, out unHi, out unLo);
				box[nI++] = unHi;
				box[nI++] = unLo;
			}
			box = m_sbox4;
			for (nI = 0; nI < SBOX_ENTRIES;)
			{
				EncryptBlock(unHi, unLo, out unHi, out unLo);
				box[nI++] = unHi;
				box[nI++] = unLo;
			}
		}

		//////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Encrypts a single block.
		/// </summary>
		/// <param name="unHi">high 32bit word of the block</param>
		/// <param name="unLo">low 32bit word of the block</param>
		/// <param name="unOutHi">where to put the encrypted high word</param>
		/// <param name="unOutLo">where to put the encrypted low word</param>
		public void EncryptBlock(uint unHi, uint unLo, out uint unOutHi, out uint unOutLo)
		{
			byte[] block;
			block = m_block;

			block[0] = (byte)(unHi >> 24);
			block[1] = (byte)(unHi >> 16);
			block[2] = (byte)(unHi >> 8);
			block[3] = (byte)unHi;
			block[4] = (byte)(unLo >> 24);
			block[5] = (byte)(unLo >> 16);
			block[6] = (byte)(unLo >> 8);
			block[7] = (byte)unLo;

			Encrypt(block, 0, block, 0, BLOCK_SIZE);

			unOutHi = (((uint)block[0]) << 24) |
				(((uint)block[1]) << 16) |
				(((uint)block[2]) << 8) |
				block[3];

			unOutLo = (((uint)block[4]) << 24) |
				(((uint)block[5]) << 16) |
				(((uint)block[6]) << 8) |
				block[7];
		}

		//////////////////////////////////////////////////////////////////////
		/// <summary>
		///   Encrypts byte buffers
		/// </summary>
		/// <remarks>
		///   Use this method to encrypt bytes from one array to another one.
		///   You can also use the same array for input and output. Note that
		///   the number of bytes must be adjusted to the block size of the
		///   algorithm. Overlapping bytes will not be encrypted. No check for
		///   buffer overflows are made.
		/// </remarks>
		/// <param name="dataIn"> input buffer </param>
		/// <param name="nPosIn"> where to start reading in the input buffer </param>
		/// <param name="dataOut"> output buffer </param>
		/// <param name="nPosOut"> where to start writing to the output buffer </param>
		/// <param name="nCount"> number ob bytes to encrypt </param>
		/// <returns>
		///   number of bytes processed
		/// </returns>
		public int Encrypt(byte[] dataIn, int nPosIn, byte[] dataOut, int nPosOut, int nCount)
		{
			int nEnd;
			uint unHi, unLo;

			uint[] sbox1 = m_sbox1;
			uint[] sbox2 = m_sbox2;
			uint[] sbox3 = m_sbox3;
			uint[] sbox4 = m_sbox4;

			uint[] pbox = m_pbox;

			uint pbox00 = pbox[0];
			uint pbox01 = pbox[1];
			uint pbox02 = pbox[2];
			uint pbox03 = pbox[3];
			uint pbox04 = pbox[4];
			uint pbox05 = pbox[5];
			uint pbox06 = pbox[6];
			uint pbox07 = pbox[7];
			uint pbox08 = pbox[8];
			uint pbox09 = pbox[9];
			uint pbox10 = pbox[10];
			uint pbox11 = pbox[11];
			uint pbox12 = pbox[12];
			uint pbox13 = pbox[13];
			uint pbox14 = pbox[14];
			uint pbox15 = pbox[15];
			uint pbox16 = pbox[16];
			uint pbox17 = pbox[17];

			nCount &= ~(BLOCK_SIZE - 1);

			nEnd = nPosIn + nCount;

			while (nPosIn < nEnd)
			{
				unHi = (((uint)dataIn[nPosIn]) << 24) |
					(((uint)dataIn[nPosIn + 1]) << 16) |
					(((uint)dataIn[nPosIn + 2]) << 8) |
					dataIn[nPosIn + 3];

				unLo = (((uint)dataIn[nPosIn + 4]) << 24) |
					(((uint)dataIn[nPosIn + 5]) << 16) |
					(((uint)dataIn[nPosIn + 6]) << 8) |
					dataIn[nPosIn + 7];
				nPosIn += 8;

				unHi ^= pbox00;
				unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^ sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox01;
				unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^ sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox02;
				unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^ sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox03;
				unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^ sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox04;
				unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^ sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox05;
				unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^ sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox06;
				unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^ sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox07;
				unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^ sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox08;
				unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^ sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox09;
				unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^ sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox10;
				unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^ sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox11;
				unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^ sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox12;
				unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^ sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox13;
				unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^ sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox14;
				unLo ^= (((sbox1[(int)(unHi >> 24)] + sbox2[(int)((unHi >> 16) & 0x0ff)]) ^ sbox3[(int)((unHi >> 8) & 0x0ff)]) + sbox4[(int)(unHi & 0x0ff)]) ^ pbox15;
				unHi ^= (((sbox1[(int)(unLo >> 24)] + sbox2[(int)((unLo >> 16) & 0x0ff)]) ^ sbox3[(int)((unLo >> 8) & 0x0ff)]) + sbox4[(int)(unLo & 0x0ff)]) ^ pbox16;

				unLo ^= pbox17;

				dataOut[nPosOut] = (byte)(unLo >> 24);
				dataOut[nPosOut + 1] = (byte)(unLo >> 16);
				dataOut[nPosOut + 2] = (byte)(unLo >> 8);
				dataOut[nPosOut + 3] = (byte)unLo;

				dataOut[nPosOut + 4] = (byte)(unHi >> 24);
				dataOut[nPosOut + 5] = (byte)(unHi >> 16);
				dataOut[nPosOut + 6] = (byte)(unHi >> 8);
				dataOut[nPosOut + 7] = (byte)unHi;

				nPosOut += 8;
			}

			return nCount;
		}
		#endregion

		public WemadeCrypt(string key)
        {
			if (key == null || key.Length == 0)
				throw new ArgumentNullException("key", "key must be a valid string");

			this.key = Encoding.ASCII.GetBytes(key.ToCharArray());
			Initialize(this.key, 0, 0);
        }

        public void BlowFish_DecryMem(byte[] Src, int SrcLen)
        {
            if (Src == null || SrcLen < 8 || SrcLen % 8 != 0)
                return;

            int blocks = SrcLen / 8;
            for (int i = 0; i < blocks; i++)
            {
                // 读取当前块的两个DWORD（小端序）
                uint firstDword = BitConverter.ToUInt32(Src, i * 8);
                uint nextDword = BitConverter.ToUInt32(Src, i * 8 + 4);

                // 16轮Feistel网络
                for (int j = 0; j < 16; j++)
                {
                    uint xorCode = m_pbox[17 - j];
                    firstDword ^= xorCode;

                    // 分解为4个字节
                    byte b1 = (byte)(firstDword >> 24);
                    byte b2 = (byte)(firstDword >> 16);
                    byte b3 = (byte)(firstDword >> 8);
                    byte b4 = (byte)firstDword;

                    // F函数处理
                    uint e1 = m_sbox1[b1];
                    uint e2 = m_sbox2[b2];
                    e2 += e1;
                    uint e3 = m_sbox3[b3];
                    e3 ^= e2;
                    uint e4 = m_sbox4[b4];
                    e4 += e3;

                    // 交换并异或
                    uint temp = nextDword ^ e4;
                    nextDword = firstDword;
                    firstDword = temp;
                }

                // 最后处理并交换位置
                firstDword ^= m_pbox[1];
                nextDword ^= m_pbox[0];

                // 写回结果（小端序）
                byte[] nextBytes = BitConverter.GetBytes(nextDword);
                byte[] firstBytes = BitConverter.GetBytes(firstDword);
                Array.Copy(nextBytes, 0, Src, i * 8, 4);
                Array.Copy(firstBytes, 0, Src, i * 8 + 4, 4);
            }
        }

        public byte[] BlowFish_EncryMem(byte[] src, int SrcLen)
        {
            const byte Fill_Char = 20; // 根据实际情况调整填充字符
            if (src == null || SrcLen < 0)
                return null;

            int newLen = (SrcLen + 7) & ~0x7; // 调整为8的倍数
            if (newLen < 8)
                return null;

            byte[] result = new byte[newLen];
			//使用循环填充
			for (int i = src.Length; i < newLen; i++)
			{
				result[i] = Fill_Char;
			}
			Array.Copy(src, result, SrcLen);

            for (int i = 0; i < newLen / 8; i++)
            {
                int offset = i * 8;
                // 小端序读取DWORD
                uint firstDword = BitConverter.ToUInt32(result, offset);
                uint nextDword = BitConverter.ToUInt32(result, offset + 4);

                // 初始异或操作
                firstDword ^= m_pbox[0];
                nextDword ^= m_pbox[1];

                for (int j = 0; j < 16; j++)
                {
                    if (j > 0)
                    {
                        uint xorCode = m_pbox[2 + (j - 1)];
                        nextDword ^= xorCode;
                    }

                    // 分解字节
                    byte B1 = (byte)((firstDword >> 24) & 0xFF);
                    byte B2 = (byte)((firstDword >> 16) & 0xFF);
                    byte B3 = (byte)((firstDword >> 8) & 0xFF);
                    byte B4 = (byte)(firstDword & 0xFF);

                    // F函数计算
                    uint E1 = m_sbox1[B1];
                    uint E2 = m_sbox2[B2];
                    E2 += E1;
                    uint E3 = m_sbox3[B3];
                    E3 ^= E2;
                    uint E4 = m_sbox4[B4];
                    E4 += E3;

                    uint tempCar = E4 ^ nextDword;
                    nextDword = firstDword;
                    firstDword = tempCar;
                }

                // 最终异或并交换
                nextDword ^= m_pbox[17];
                // 小端序写入
                byte[] nextBytes = BitConverter.GetBytes(nextDword);
                byte[] firstBytes = BitConverter.GetBytes(firstDword);

                Array.Copy(nextBytes, 0, result, offset, 4);
                Array.Copy(firstBytes, 0, result, offset + 4, 4);
            }

            return result;
        }
	}
}
