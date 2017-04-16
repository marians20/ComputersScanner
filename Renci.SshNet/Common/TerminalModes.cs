namespace Renci.SshNet.Common
{
    /// <summary>
    /// Specifies the initial assignments of the opcode values that are used in the 'encoded terminal modes' valu
    /// </summary>
    public enum TerminalModes : byte
    {
        /// <summary>
        /// Indicates end of options.
        /// </summary> 
        TtyOpEnd = 0,
        
        /// <summary>
        /// Interrupt character; 255 if none.  Similarly for the other characters.  Not all of these characters are supported on all systems.
        /// </summary> 
        Vintr = 1,

        /// <summary>
        /// The quit character (sends SIGQUIT signal on POSIX systems).
        /// </summary> 
        Vquit = 2,
        
        /// <summary>
        /// Erase the character to left of the cursor. 
        /// </summary>
        Verase = 3,

        /// <summary>
        /// Kill the current input line.
        /// </summary>
        Vkill = 4,

        /// <summary>
        /// End-of-file character (sends EOF from the terminal).
        /// </summary>
        Veof = 5,
        
        /// <summary>
        /// End-of-line character in addition to carriage return and/or linefeed.
        /// </summary>
        Veol = 6,
        
        /// <summary>
        /// Additional end-of-line character.
        /// </summary>
        Veol2 = 7,
        
        /// <summary>
        /// Continues paused output (normally control-Q).
        /// </summary>
        Vstart = 8,
        
        /// <summary>
        /// Pauses output (normally control-S).
        /// </summary>
        Vstop = 9,
        
        /// <summary>
        /// Suspends the current program.
        /// </summary>
        Vsusp = 10,
        
        /// <summary>
        /// Another suspend character.
        /// </summary>
        Vdsusp = 11,

        /// <summary>
        /// Reprints the current input line.
        /// </summary>
        Vreprint = 12,

        /// <summary>
        /// Erases a word left of cursor.
        /// </summary>
        Vwerase = 13,

        /// <summary>
        /// Enter the next character typed literally, even if it is a special character
        /// </summary>
        Vlnext = 14,

        /// <summary>
        /// Character to flush output.
        /// </summary>
        Vflush = 15,

        /// <summary>
        /// Switch to a different shell layer.
        /// </summary>
        Vswtch = 16,

        /// <summary>
        /// Prints system status line (load, command, pid, etc).
        /// </summary>
        Vstatus = 17,

        /// <summary>
        /// Toggles the flushing of terminal output.
        /// </summary>
        Vdiscard = 18,

        /// <summary>
        /// The ignore parity flag.  The parameter SHOULD be 0 if this flag is FALSE, and 1 if it is TRUE.
        /// </summary>
        Ignpar = 30,

        /// <summary>
        /// Mark parity and framing errors.
        /// </summary>
        Parmrk = 31,

        /// <summary>
        /// Enable checking of parity errors.
        /// </summary>
        Inpck = 32,

        /// <summary>
        /// Strip 8th bit off characters.
        /// </summary>
        Istrip = 33,

        /// <summary>
        /// Map NL into CR on input.
        /// </summary>
        Inlcr = 34,

        /// <summary>
        /// Ignore CR on input.
        /// </summary>
        Igncr = 35,

        /// <summary>
        /// Map CR to NL on input.
        /// </summary>
        Icrnl = 36,

        /// <summary>
        /// Translate uppercase characters to lowercase.
        /// </summary>
        Iuclc = 37,

        /// <summary>
        /// Enable output flow control.
        /// </summary>
        Ixon = 38,

        /// <summary>
        /// Any char will restart after stop.
        /// </summary>
        Ixany = 39,

        /// <summary>
        /// Enable input flow control.
        /// </summary>
        Ixoff = 40,

        /// <summary>
        /// Ring bell on input queue full.
        /// </summary>
        Imaxbel = 41,

        /// <summary>
        /// Enable signals INTR, QUIT, [D]SUSP.
        /// </summary>
        Isig = 50,

        /// <summary>
        /// Canonicalize input lines.
        /// </summary>
        Icanon = 51,

        /// <summary>
        /// Enable input and output of uppercase characters by preceding their lowercase equivalents with "\".
        /// </summary>
        Xcase = 52,

        /// <summary>
        /// Enable echoing.
        /// </summary>
        Echo = 53,

        /// <summary>
        /// Visually erase chars.
        /// </summary>
        Echoe = 54,

        /// <summary>
        /// Kill character discards current line.
        /// </summary>
        Echok = 55,

        /// <summary>
        /// Echo NL even if ECHO is off.
        /// </summary>
        Echonl = 56,

        /// <summary>
        /// Don't flush after interrupt.
        /// </summary>
        Noflsh = 57,

        /// <summary>
        /// Stop background jobs from output.
        /// </summary>
        Tostop = 58,

        /// <summary>
        /// Enable extensions.
        /// </summary>
        Iexten = 59,

        /// <summary>
        /// Echo control characters as ^(Char).
        /// </summary>
        Echoctl = 60,

        /// <summary>
        /// Visual erase for line kill.
        /// </summary>
        Echoke = 61,

        /// <summary>
        /// Retype pending input.
        /// </summary>
        Pendin = 62,

        /// <summary>
        /// Enable output processing.
        /// </summary>
        Opost = 70,

        /// <summary>
        /// Convert lowercase to uppercase.
        /// </summary>
        Olcuc = 71,

        /// <summary>
        /// Map NL to CR-NL.
        /// </summary>
        Onlcr = 72,

        /// <summary>
        /// Translate carriage return to newline (output).
        /// </summary>
        Ocrnl = 73,

        /// <summary>
        /// Translate newline to carriage return-newline (output).
        /// </summary>
        Onocr = 74,

        /// <summary>
        /// Newline performs a carriage return (output).
        /// </summary>
        Onlret = 75,

        /// <summary>
        /// 7 bit mode.
        /// </summary>
        Cs7 = 90,

        /// <summary>
        /// 8 bit mode.
        /// </summary>
        Cs8 = 91,

        /// <summary>
        /// Parity enable.
        /// </summary>
        Parenb = 92,

        /// <summary>
        /// Odd parity, else even.
        /// </summary>
        Parodd = 93,

        /// <summary>
        /// Specifies the input baud rate in bits per second.
        /// </summary>
        TtyOpIspeed = 128,

        /// <summary>
        /// Specifies the output baud rate in bits per second.
        /// </summary>
        TtyOpOspeed = 129,
    }
}
