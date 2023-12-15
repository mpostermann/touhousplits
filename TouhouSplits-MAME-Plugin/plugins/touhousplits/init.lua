-- license:BSD-3-Clause
-- copyright-holders:MPOstermann
-- Something something I hope this works
require 'winapi'

local exports = {}
exports.name = "touhousplits"
exports.version = "0.0.1"
exports.description = "Touhou Splits Plugin"
exports.license = "BSD-3-Clause"
exports.author = { name = "MPOstermann" }

local touhousplits = exports

local cpu
local mem

local pipeFile
local readyToWrite = true
local currentFrame = 0
local framesBetweenWrite = 2

local supportedRoms = {	["donpachij"] = true }

function touhousplits.set_folder(path)
	touhousplits.path = path
end

local pipe = winapi.make_pipe_server(function(file, error)
	if error then
		emu.print_info("Error occurred while establishing pipe: " .. error)
	else
		emu.print_info("Client connected")
		pipeFile = file

		-- while true do
		-- 	if mem ~= nil and file ~= nil then
		-- 		local score = mem:read_u32(tonumber("10177A", 16))
		-- 		local success, errorMsg = pcall(function() return file:write(string.format("%08x", score) .. "\n") end)
		-- 		if not success then
		-- 			emu.print_info("Error writing to pipe: " .. errorMsg)
		-- 		end

		-- 		winapi.sleep(40)
		-- 	end
		-- end
	end
end)

function touhousplits.startplugin()

	emu.register_start(function()
		if supportedRoms[emu.romname()] then
			emu.print_info("Loading script for " .. emu.romname())
			cpu = manager.machine.devices["maincpu"];
			if not cpu then
				emu.print_error("maincpu device not found")
			else
				mem = cpu.spaces["program"]
				if not mem then
					error("program space not found")
				end
			end
		end
	end)

	emu.register_frame(function()
		if readyToWrite then
			if mem ~= nil and pipeFile ~= nil and currentFrame > framesBetweenWrite then
				readyToWrite = false
				local score = mem:read_u32(tonumber("10177A", 16))
				local success, errorMsg = pcall(function() return pipeFile:write(string.format("%08x", score) .. "\n") end) -- TODO: Make this non-blocking
				if not success then
					emu.print_info("Error writing to pipe: " .. errorMsg)
				end

				readyToWrite = true
				currentFrame = 0
			end
			currentFrame = currentFrame + 1
		end
	end)

	emu.register_stop(function()
		mem = nil
		cpu = nil
	end)
end

return exports
