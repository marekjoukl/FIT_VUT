-- uart_rx.vhd: UART controller - receiving (RX) side
-- Author(s): Marek Joukl (xjoukl00)

library ieee;
use ieee.std_logic_1164.all;
use ieee.std_logic_unsigned.all;

-- Entity declaration (DO NOT ALTER THIS PART!)
entity UART_RX is
    port(
        CLK      : in std_logic;
        RST      : in std_logic;
        DIN      : in std_logic;
        DOUT     : out std_logic_vector(7 downto 0);
        DOUT_VLD : out std_logic
    );
end entity;

-- Architecture implementation (INSERT YOUR IMPLEMENTATION HERE)
architecture behavioral of UART_RX is
    signal cnt_1    : std_logic_vector(4 downto 0) := "00000";
    signal cnt_2    : std_logic_vector(3 downto 0) := "0000";
    signal rec_en   : std_logic;
    signal cnt_en   : std_logic;
    signal d_vld    : std_logic;
begin

    -- Instance of RX FSM
    fsm: entity work.UART_RX_FSM
    port map (
        CLK      => CLK,
        RST      => RST,
        DIN      => DIN,
        CNT_1    => cnt_1,
        CNT_2    => cnt_2,
        REC_EN   => rec_en,
        CNT_EN   => cnt_en,
        DOUT_VLD => d_vld
    );

    DOUT_VLD <= d_vld;

    process (CLK)
    begin
        if rising_edge(CLK) then
            if cnt_en = '1' then
                cnt_1 <= cnt_1 + 1;
            else 
                cnt_1 <= "00000";
                cnt_2 <= "0000";
            end if ;

            if rec_en = '1' then
                if cnt_1 = "10000" then
                    cnt_1 <= "00000";
                    case cnt_2 is
                        when "0000" => DOUT(0) <= DIN;
                        when "0001" => DOUT(1) <= DIN;
                        when "0010" => DOUT(2) <= DIN;
                        when "0011" => DOUT(3) <= DIN;
                        when "0100" => DOUT(4) <= DIN;
                        when "0101" => DOUT(5) <= DIN;
                        when "0110" => DOUT(6) <= DIN;
                        when "0111" => DOUT(7) <= DIN; 
                        when others => null;
                    end case;
                    cnt_2 <= cnt_2 + 1;
                end if ;
            end if;
            if d_vld = '1' then
                DOUT <= "00000000";
            end if ;
        end if;
    end process;
end architecture;
