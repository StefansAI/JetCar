# SPDX-FileCopyrightText: 2021 Stefan Warnke
#
# SPDX-License-Identifier: BeerWare

"""
`SimpleTiming`
==========

This module contains a definition of a simple time measurement class. 
Times will be captured and deltas will be provided to a previous event 
in the same cycle. A history will be kept for later evaluation.

* Author(s): Stefan Warnke

Dependency:
    - time
    - jetcar_definitions
"""

import time
from jetcar_definitions import *

TIME_MS = 1000
TIME_ACC = 10
TIME_SCALE = TIME_MS/TIME_ACC

class SimpleTiming:
    """ The goal of this class is measuring different time deltas depending on the 
    requested array size. For instance an array size of 6 will result in 5 time deltas 
    per cycle. The history_size defines the depth of the storage or the number of cycles 
    kept in memory"""
    def __init__(self, array_size, hist_size):
        """Creates and initializes the SimpleTiming object.
        array_size  -- Number of time captures per cycle.
        hist_size   -- Number of cycles to keep in memory"""
        self.array_size = array_size
        self.hist_size = hist_size
        self.time_sample=[0 for _ in range(array_size)]
        self.time_delta=[0 for _ in range(array_size)]
        self.dt_acc=[0 for _ in range(array_size)]
        self.dt_avg=[0 for _ in range(array_size)]
        self.count = 0
        self.hist = []
        self.cidx = -1
        self.didx = -1
        
    def measure_time(self, current_idx, delta_idx):
        """Captures the time of the call, stores under the current index 
        in this cycle and calculates the delta to the request previous 
        index of the same cycle.
        current_idx  -- Index to store the time in the array
        delta_idx    -- Index of the previous entry to calculate the delta from"""
        t = time.perf_counter()
        dt = t-self.time_sample[delta_idx]
        self.time_delta[current_idx] = dt
        self.dt_acc[current_idx] += dt
        self.time_sample[current_idx] = t
        self.cidx = current_idx
        self.didx = delta_idx

    def loop_end(self):
        """Handle the end of the cycle. Add the current array to the 
        history buffer if not full yet. Provide some averaging over a 
        number of cycles for updating a display value with more stable values.
        returns  -- True, when averaging cycle is done."""
        if len(self.hist)<self.hist_size:
            self.hist.append(self.time_delta.copy())
        self.count += 1
        if self.count >= TIME_ACC:
            self.count = 0
            for i in range(self.array_size):
                self.dt_avg[i] = self.dt_acc[i] * TIME_SCALE
                self.dt_acc[i] = 0
            return True
        else:
            return False
        
    def get_str(self):
        """Creates a string representation of the last time delta averages."""
        s = ""
        for i in range(self.array_size):
            s += "dt%d=%.1f " % (i,self.dt_avg[i])

        return s
        
    def get_hist_str(self, hist_idx):
        """Creates a string representation of one cycel in the history buffer.
        hist_idx  -- Index in the history buffer for the entry to be converted"""
        s = ""
        for i in range(self.array_size):
            s += "dt%d=%.1f " % (i,self.hist[hist_idx][i] * TIME_MS)
        return s
        
        