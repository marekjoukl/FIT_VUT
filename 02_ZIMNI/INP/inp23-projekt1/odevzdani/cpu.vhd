-- cpu.vhd: Simple 8-bit CPU (BrainFuck interpreter)
-- Copyright (C) 2023 Brno University of Technology,
--                    Faculty of Information Technology
-- Author(s): Marek Joukl <xjoukl00 AT stud.fit.vutbr.cz>
--
library ieee;
use ieee.std_logic_1164.all;
use ieee.std_logic_arith.all;
use ieee.std_logic_unsigned.all;

-- ----------------------------------------------------------------------------
--                        Entity declaration
-- ----------------------------------------------------------------------------
entity cpu is
 port (
   CLK   : in std_logic;  -- hodinovy signal
   RESET : in std_logic;  -- asynchronni reset procesoru
   EN    : in std_logic;  -- povoleni cinnosti procesoru
 
   -- synchronni pamet RAM
   DATA_ADDR  : out std_logic_vector(12 downto 0); -- adresa do pameti
   DATA_WDATA : out std_logic_vector(7 downto 0); -- mem[DATA_ADDR] <- DATA_WDATA pokud DATA_EN='1'
   DATA_RDATA : in std_logic_vector(7 downto 0);  -- DATA_RDATA <- ram[DATA_ADDR] pokud DATA_EN='1'
   DATA_RDWR  : out std_logic;                    -- cteni (0) / zapis (1)
   DATA_EN    : out std_logic;                    -- povoleni cinnosti
   
   -- vstupni port
   IN_DATA   : in std_logic_vector(7 downto 0);   -- IN_DATA <- stav klavesnice pokud IN_VLD='1' a IN_REQ='1'
   IN_VLD    : in std_logic;                      -- data platna
   IN_REQ    : out std_logic;                     -- pozadavek na vstup data
   
   -- vystupni port
   OUT_DATA : out  std_logic_vector(7 downto 0);  -- zapisovana data
   OUT_BUSY : in std_logic;                       -- LCD je zaneprazdnen (1), nelze zapisovat
   OUT_WE   : out std_logic;                      -- LCD <- OUT_DATA pokud OUT_WE='1' a OUT_BUSY='0'

   -- stavove signaly
   READY    : out std_logic;                      -- hodnota 1 znamena, ze byl procesor inicializovan a zacina vykonavat program
   DONE     : out std_logic                       -- hodnota 1 znamena, ze procesor ukoncil vykonavani programu (narazil na instrukci halt)
 );
end cpu;


-- ----------------------------------------------------------------------------
--                      Architecture declaration
-- ----------------------------------------------------------------------------
architecture behavioral of cpu is
  -- PC
  signal pc_reg : std_logic_vector(12 downto 0);
  signal pc_dec : std_logic;
  signal pc_inc : std_logic;


  -- PTR
  signal ptr_reg : std_logic_vector(12 downto 0);
  signal ptr_dec : std_logic;
  signal ptr_inc : std_logic;

  -- CNT
  signal cnt_reg : std_logic_vector(7 downto 0);
  signal cnt_dec : std_logic;
  signal cnt_inc : std_logic;

  signal mx_1_sel : std_logic;

  signal mx_2_sel : std_logic_vector(1 downto 0);

  type fsm_state is (sidle, sfetch0, sfetch1, sdecode, shalt, snop, s_ptr_inc, s_ptr_dec,
                     s_data_inc1, s_data_inc2, s_data_dec1, s_data_dec2, s_start_while, 
                     s_end_while1, s_end_while2, s_end_while3, s_end_while4, s_end_while5, s_break1, s_break2, s_break3, 
                     s_print1, s_print2, s_load1, s_load2, s_others, s_cont_while1, s_cont_while2, s_cont_while3,
                     s_wait_while0, s_wait_while1, s_wait_while2);

  type instruc_type is (d_ptr_inc, d_ptr_dec, d_data_inc, d_data_dec, d_start_while,
                        d_end_while, d_break, d_print, d_load, d_halt, d_others);

  signal instruction : instruc_type;
                  
  signal pstate : fsm_state;
  signal nstate : fsm_state;

begin

 -- pri tvorbe kodu reflektujte rady ze cviceni INP, zejmena mejte na pameti, ze 
 --   - nelze z vice procesu ovladat stejny signal,
 --   - je vhodne mit jeden proces pro popis jedne hardwarove komponenty, protoze pak
 --      - u synchronnich komponent obsahuje sensitivity list pouze CLK a RESET a 
 --      - u kombinacnich komponent obsahuje sensitivity list vsechny ctene signaly. 

-- Program counter PC
pc_cntr: process (RESET, CLK, pc_inc, pc_dec)
begin
  if (RESET='1') then
    pc_reg <= (others=>'0');
  elsif (CLK'event) and (CLK='1') then
    if (pc_inc='1') then
      pc_reg <= (pc_reg + 1);
    elsif (pc_dec='1') then
      pc_reg <= (pc_reg - 1);
    end if;
  end if;
end process pc_cntr;

-- PTR
ptr_cntr: process (RESET, CLK, ptr_inc, ptr_dec)
begin
  if (RESET='1') then
    ptr_reg <= (others=>'0');
  elsif (CLK'event) and (CLK='1') then
    if (ptr_inc = '1') then
      ptr_reg <= (ptr_reg + 1);
    elsif (ptr_dec = '1') then
      ptr_reg <= (ptr_reg - 1);
    end if;
  end if;
end process ptr_cntr;


-- CNT
cnt_cntr: process (RESET, CLK, cnt_inc, cnt_dec)
begin
  if (RESET='1') then
    cnt_reg <= (others=>'0');
  elsif (CLK'event) and (CLK='1') then
    if (cnt_dec='1') then
      cnt_reg <= (cnt_reg - 1);
    elsif (cnt_inc='1') then
      cnt_reg <= (cnt_reg + 1);
    end if;
  end if;
end process cnt_cntr;

-- MULTIPLEXOR 1
DATA_ADDR <= ptr_reg when mx_1_sel = '0' else pc_reg;

-- MULTIPLEXOR 2
DATA_WDATA <= IN_DATA        when mx_2_sel = "00" else
              DATA_RDATA - 1 when mx_2_sel = "01" else
              DATA_RDATA + 1 when mx_2_sel = "10" else
              IN_DATA;  

decoder: process (DATA_RDATA)
begin
  case( DATA_RDATA ) is
    when x"3E" => instruction <= d_ptr_inc;
    when x"3C" => instruction <= d_ptr_dec;
    when x"2B" => instruction <= d_data_inc;
    when x"2D" => instruction <= d_data_dec;
    when x"5B" => instruction <= d_start_while;
    when x"5D" => instruction <= d_end_while;
    when x"7E" => instruction <= d_break;
    when x"2E" => instruction <= d_print;
    when x"2C" => instruction <= d_load;
    when x"40" => instruction <= d_halt;
    when others => instruction <= d_others;
  end case ;
end process;

fsm_pstate: process (RESET, CLK)
  begin
    if (RESET='1') then
      pstate <= sidle;
    elsif (CLK'event) and (CLK='1') then
      if (EN = '1') then
        pstate <= nstate;
      end if;
    end if;
end process;

fsm_nstate: process(pstate, DATA_RDATA, OUT_BUSY, IN_VLD, EN, IN_DATA, instruction)
begin
  case (pstate) is
    when sidle =>
      DATA_EN <= '0';
      DATA_RDWR <= '0';
      OUT_WE <= '0';
      IN_REQ <= '0';
      mx_2_sel <= "00";
      mx_1_sel <= '0';
      ptr_inc <= '0';
      ptr_dec <= '0';
      pc_inc <= '0';
      pc_dec <= '0';
      cnt_inc <= '0';
      cnt_dec <= '0';
      
      DONE <= '0';  
      READY <= '0';
      nstate <= sfetch0;

    when sfetch0 =>
      DATA_EN <= '1';
      DATA_RDWR <= '0';
      mx_1_sel <= '0';
      if instruction = d_halt then
        nstate <= sfetch1;
        ptr_inc <= '0';
        READY <= '1';
      else
        ptr_inc <= '1';
        nstate <= sfetch0;
      end if;
  
    when sfetch1 =>
      DATA_RDWR <= '0';
      pc_inc <= '0';
      pc_dec <= '0';
      mx_1_sel <= '1';
      ptr_inc <= '0';
      ptr_dec <= '0';
      mx_2_sel <= "00";
      IN_REQ <= '0';
      OUT_WE <= '0';
      cnt_inc <= '0';
      cnt_dec <= '0';
      nstate <= sdecode;

    when sdecode =>
      case (instruction) is        
        -- ">"
        when d_ptr_inc =>
          nstate <= s_ptr_inc;

        -- "<"
        when d_ptr_dec =>
          nstate <= s_ptr_dec;
          
        -- "+"
        when d_data_inc =>
          nstate <= s_data_inc1;

        -- "-"
        when d_data_dec =>
          nstate <= s_data_dec1;

        -- "["
        when d_start_while =>
          nstate <= s_start_while;
    
        -- "]"
        when d_end_while =>
          nstate <= s_end_while1;

        -- "~"
        when d_break =>
          nstate <= s_break1;

        -- "."
        when d_print =>
          nstate <= s_print1;

        -- ","
        when d_load =>
          nstate <= s_load1;
          
        -- @
        when d_halt =>
          nstate <= shalt;

        when others => 
          nstate <= s_others;
      end case ;
      
    when s_ptr_inc =>
      pc_inc <= '1';
      ptr_inc <= '1';
      nstate <= sfetch1;

    when s_ptr_dec =>
      pc_inc <= '1';
      ptr_dec <= '1';
      nstate <= sfetch1;
    
    when s_data_inc1 =>
      mx_1_sel <= '0';
      mx_2_sel <= "10";
      DATA_RDWR <= '0';
      nstate <= s_data_inc2;

    when s_data_inc2 =>
      DATA_RDWR <= '1';
      pc_inc <= '1';
      nstate <= sfetch1;

    when s_data_dec1 =>
      mx_1_sel <= '0';
      mx_2_sel <= "01";
      DATA_RDWR <= '0';
      nstate <= s_data_dec2;

    when s_data_dec2 =>
      DATA_RDWR <= '1';
      pc_inc <= '1';
      nstate <= sfetch1;

    when s_start_while =>
      mx_1_sel <= '0';
      pc_inc <= '1';
      nstate <= s_cont_while1;
      
    when s_cont_while1 =>
      pc_inc <= '0';
      if (DATA_RDATA = "00000000") then
        cnt_inc <= '1';
        nstate <= s_cont_while2;
      else
        nstate <= sfetch1;
      end if ;
    
    when s_cont_while2 =>
      cnt_inc <= '0';
      cnt_dec <= '0';
      pc_inc <= '0';
      mx_1_sel <= '1';
      nstate <= s_wait_while0;

    when s_wait_while0 =>
      nstate <= s_cont_while3;

    when s_cont_while3 =>
      if (cnt_reg /= "00000000") then
        if instruction = d_start_while then   -- "["
          cnt_inc <= '1';
        elsif instruction = d_end_while then -- "]"
          cnt_dec <= '1';
        end if ;
        pc_inc <= '1';
        nstate <= s_cont_while2;
      else
        pc_inc <= '0';
        pc_dec <= '0';
        cnt_inc <= '0';
        cnt_dec <= '0';
        nstate <= sfetch1;
      end if ;

    when s_end_while1 =>
      mx_1_sel <= '0';
      nstate <= s_wait_while1;

    when s_wait_while1 =>
      nstate <= s_end_while2;

    when s_end_while2 =>
      if (DATA_RDATA = "00000000") then
        pc_inc <= '1';
        nstate <= sfetch1;
      else
        cnt_inc <= '1';
        pc_dec <= '1';
        mx_1_sel <= '1';
        nstate <= s_end_while3;
      end if ;

    when s_end_while3 =>
    if cnt_reg /= 0 then
      pc_inc <= '0';
      pc_dec <= '0';
      cnt_inc <= '0';
      cnt_dec <= '0';
      nstate <= s_end_while4;
    else
      nstate <= sfetch1;
    end if ;

    when s_end_while4 =>
      pc_inc <= '0';
      pc_dec <= '0';
      cnt_inc <= '0';
      cnt_dec <= '0';
      if (cnt_reg /= "00000000") then
        if (instruction = d_end_while) then -- ']'
          cnt_inc <= '1';
        elsif (instruction = d_start_while) then -- '['
          cnt_dec <= '1';
        end if;
        nstate <= s_wait_while2;
      else
        nstate <= sfetch1;
      end if;

    when s_wait_while2 =>
      cnt_inc <= '0';
      cnt_dec <= '0';  
      nstate <= s_end_while5;

    when s_end_while5 =>
      if (cnt_reg = "00000000") then 
        pc_inc <= '1';  
        nstate <= sfetch1;
      else
        pc_dec <= '1';
        nstate <= s_end_while3;
      end if;

    when s_break1 =>
      pc_inc <= '1';
      nstate <= s_break2;

    when s_break2 =>
      pc_inc <= '1';
      if cnt_reg = "00000000" then
        nstate <= sfetch1;
      else
        if DATA_RDATA = x"5B" then
          cnt_inc <= '1';
        elsif DATA_RDATA = x"5D" then
          cnt_dec <= '1';
        end if ;
      nstate <= s_break3;
      end if ;

    when s_break3 =>
      pc_inc <= '0';
      cnt_inc <= '0';
      cnt_dec <= '0';
      nstate <= s_break2;

    when s_print1 =>
      mx_1_sel <= '0';
      nstate <= s_print2;

    when s_print2 =>
      if (OUT_BUSY = '1') then
        nstate <= s_print1;
      else
        OUT_DATA <= DATA_RDATA;
        OUT_WE <= '1';
        pc_inc <= '1';
        nstate <= sfetch1;
      end if ;

    when s_load1 =>
      IN_REQ <= '1';
      if IN_VLD = '0' then
        nstate <= s_load1;
      else
        nstate <= s_load2;
      end if ;

    when s_load2 =>
      mx_1_sel <= '0';
      DATA_RDWR <= '1';
      pc_inc <= '1';
      nstate <= sfetch1;

    when shalt =>
      DONE <= '1';
      nstate <= shalt;

    when s_others =>
      pc_inc <= '1';
      nstate <= sfetch1;
    
    when others =>
      pc_inc <= '1';
      nstate <= sfetch1;
  end case ;
end process fsm_nstate;

end behavioral;

