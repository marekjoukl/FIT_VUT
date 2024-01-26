# Google Tests makefile

TARGET = mathTests.out

# Compiler settings
CXX = g++
CXXFLAGS = -std=c++14 -Wall -Wextra

# Directories
GTEST_DIR = ./googletest

# Object file
OBJS = $(patsubst %.cpp, %.o, $(wildcard *.cpp))

# Include directories
INC = -I$(GTEST_DIR)/googletest/include \
    -I$(GTEST_DIR)/googlemock/include

# Libraries to link
LIBS = $(GTEST_DIR)/build/lib/libgtest.a \
       $(GTEST_DIR)/build/lib/libgtest_main.a \
       $(GTEST_DIR)/build/lib/libgmock.a \
       $(GTEST_DIR)/build/lib/libgmock_main.a

# Targets
all: $(TARGET)

$(TARGET): $(OBJS) $(LIBS)
	$(CXX) $(CXXFLAGS) $(INC) $^ -o $@

%.o: %.cpp
	$(CXX) $(CXXFLAGS) $(INC) -c $< -o $@

$(LIBS):
	mkdir -p $(GTEST_DIR)/build
	cd $(GTEST_DIR)/build && cmake .. && make

clean:
	rm -f $(TARGET) $(OBJS)
	rm -rf $(GTEST_DIR)/build
