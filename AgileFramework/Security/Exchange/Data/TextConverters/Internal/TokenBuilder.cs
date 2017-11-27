using System;
using System.Diagnostics;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal
{
    internal abstract class TokenBuilder
    {
        protected const byte BuildStateInitialized = 0;

        protected const byte BuildStateEnded = 5;

        protected const byte FirstStarted = 10;

        protected const byte BuildStateText = 10;

        protected byte state;

        protected Token token;

        protected int maxRuns;

        protected int tailOffset;

        protected bool tokenValid;

        public Token Token
        {
            get
            {
                return token;
            }
        }

        public bool IsStarted
        {
            get
            {
                return state != 0;
            }
        }

        public bool Valid
        {
            get
            {
                return tokenValid;
            }
        }

        public TokenBuilder(Token token, char[] buffer, int maxRuns, bool testBoundaryConditions)
        {
            int num = 64;
            if (!testBoundaryConditions)
            {
                this.maxRuns = maxRuns;
            }
            else
            {
                this.maxRuns = 55;
                num = 7;
            }
            this.token = token;
            this.token.buffer = buffer;
            this.token.runList = new Token.RunEntry[num];
        }

        public void BufferChanged(char[] newBuffer, int newBase)
        {
            if (newBuffer != token.buffer || newBase != token.whole.headOffset)
            {
                token.buffer = newBuffer;
                if (newBase != token.whole.headOffset)
                {
                    int deltaOffset = newBase - token.whole.headOffset;
                    Rebase(deltaOffset);
                }
            }
        }

        public virtual void Reset()
        {
            if (state > 0)
            {
                token.Reset();
                tailOffset = 0;
                tokenValid = false;
                state = 0;
            }
        }

        public TokenId MakeEmptyToken(TokenId tokenId)
        {
            token.tokenId = tokenId;
            state = 5;
            tokenValid = true;
            return tokenId;
        }

        public TokenId MakeEmptyToken(TokenId tokenId, int argument)
        {
            token.tokenId = tokenId;
            token.argument = argument;
            state = 5;
            tokenValid = true;
            return tokenId;
        }

        public void StartText(int baseOffset)
        {
            token.tokenId = TokenId.Text;
            state = 10;
            token.whole.headOffset = baseOffset;
            tailOffset = baseOffset;
        }

        public void EndText()
        {
            state = 5;
            tokenValid = true;
            token.wholePosition.Rewind(token.whole);
            AddSentinelRun();
        }

        public bool PrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
        {
            return (start == tailOffset && token.whole.tail + numRuns < token.runList.Length) || SlowPrepareToAddMoreRuns(numRuns, start, skippedRunKind);
        }

        public bool SlowPrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
        {
            if (start != tailOffset)
            {
                numRuns++;
            }
            if (token.whole.tail + numRuns < token.runList.Length || ExpandRunsArray(numRuns))
            {
                if (start != tailOffset)
                {
                    AddInvalidRun(start, skippedRunKind);
                }
                return true;
            }
            return false;
        }

        public bool PrepareToAddMoreRuns(int numRuns)
        {
            return token.whole.tail + numRuns < token.runList.Length || ExpandRunsArray(numRuns);
        }

        [Conditional("DEBUG")]
        public void AssertPreparedToAddMoreRuns(int numRuns)
        {
        }

        [Conditional("DEBUG")]
        public void AssertCanAddMoreRuns(int numRuns)
        {
        }

        [Conditional("DEBUG")]
        public void AssertCurrentRunPosition(int position)
        {
        }

        [Conditional("DEBUG")]
        public void DebugPrepareToAddMoreRuns(int numRuns)
        {
        }

        public void AddTextRun(RunTextType textType, int start, int end)
        {
            AddRun((RunType)2147483648u, textType, 67108864u, start, end, 0);
        }

        public void AddLiteralTextRun(RunTextType textType, int start, int end, int literal)
        {
            AddRun((RunType)3221225472u, textType, 67108864u, start, end, literal);
        }

        internal void AddRun(RunType type, RunTextType textType, uint kind, int start, int end, int value)
        {
            Token.RunEntry[] arg_26_0 = token.runList;
            Token expr_16_cp_0 = token;
            int tail;
            expr_16_cp_0.whole.tail = (tail = expr_16_cp_0.whole.tail) + 1;
            arg_26_0[tail].Initialize(type, textType, kind, end - start, value);
            tailOffset = end;
        }

        internal void AddInvalidRun(int offset, uint kind)
        {
            Token.RunEntry[] arg_26_0 = token.runList;
            Token expr_16_cp_0 = token;
            int tail;
            expr_16_cp_0.whole.tail = (tail = expr_16_cp_0.whole.tail) + 1;
            arg_26_0[tail].Initialize(RunType.Invalid, RunTextType.Unknown, kind, offset - tailOffset, 0);
            tailOffset = offset;
        }

        internal void AddNullRun(uint kind)
        {
            Token.RunEntry[] arg_26_0 = token.runList;
            Token expr_16_cp_0 = token;
            int tail;
            expr_16_cp_0.whole.tail = (tail = expr_16_cp_0.whole.tail) + 1;
            arg_26_0[tail].Initialize(RunType.Invalid, RunTextType.Unknown, kind, 0, 0);
        }

        internal void AddSentinelRun()
        {
            token.runList[token.whole.tail].InitializeSentinel();
        }

        protected virtual void Rebase(int deltaOffset)
        {
            Token expr_0B_cp_0 = token;
            expr_0B_cp_0.whole.headOffset = expr_0B_cp_0.whole.headOffset + deltaOffset;
            Token expr_23_cp_0 = token;
            expr_23_cp_0.wholePosition.runOffset = expr_23_cp_0.wholePosition.runOffset + deltaOffset;
            tailOffset += deltaOffset;
        }

        private bool ExpandRunsArray(int numRuns)
        {
            int num = Math.Min(maxRuns, Math.Max(token.runList.Length * 2, token.whole.tail + numRuns + 1));
            if (num - token.whole.tail < numRuns + 1)
            {
                return false;
            }
            Token.RunEntry[] array = new Token.RunEntry[num];
            Array.Copy(token.runList, 0, array, 0, token.whole.tail);
            token.runList = array;
            return true;
        }
    }
}
