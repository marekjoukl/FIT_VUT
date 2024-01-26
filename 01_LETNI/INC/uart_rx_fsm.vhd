-- uart_rx_fsm.vhd: UART controller - finite state machine controlling RX side
-- Author(s): Marek Joukl (xjoukl00)

library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity UART_RX_FSM is
    port(
       CLK      : in std_logic;
       RST      : in std_logic;
       DIN      : in std_logic;
       CNT_1    : in std_logic_vector(4 downto 0);  --clk
       CNT_2    : in std_logic_vector(3 downto 0);  --bits
       REC_EN   : out std_logic;
       CNT_EN   : out std_logic;
       DOUT_VLD : out std_logic
    );
end entity;

architecture behavioral of UART_RX_FSM is
    type t_state is (IDLE, START_BIT, READ_DATA, STOP_BIT, VLD_DATA);
    signal cur_state : t_state := IDLE;
begin
    CNT_EN <= '1' when cur_state = START_BIT or cur_state = READ_DATA or cur_state = STOP_BIT else '0';
    REC_EN <= '1' when cur_state = READ_DATA else '0';
    DOUT_VLD <= '1' when cur_state = VLD_DATA else '0';

    proc_state : process (CLK)
    begin
        if rising_edge(CLK) then
            if RST = '1' then
                cur_state <= IDLE;
            else
                case cur_state is
                -- IDLE
                    when IDLE =>
                        if DIN = '0' then
                            cur_state <= START_BIT;
                        end if ;
                -- START BIT
                    when START_BIT => 
                        cur_state <= READ_DATA;
                -- READ_DATA
                    when READ_DATA =>
                        if CNT_2 = "1000" then
                            cur_state <= VLD_DATA;
                        end if ;
                -- STOP_BIT
                    when STOP_BIT =>
                        if DIN = '1' then
                            cur_state <= IDLE;
                        end if ;
                -- VLD_DATA
                    when VLD_DATA =>
                        cur_state <= STOP_BIT;
                -- OTHERS
                    when others => null;
                end case ;
            end if;
        end if;
    end process proc_state;
end architecture;
