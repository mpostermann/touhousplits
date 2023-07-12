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

local frameCount = 0
local supportedRoms = {	["donpachij"] = true }

function touhousplits.set_folder(path)
	touhousplits.path = path
end

local pipe = winapi.make_pipe_server(function(file, error)
	if error then
		emu.print_info("Error occurred while establishing pipe: " .. error)
	else
		emu.print_info("Client connected")

		while true do
			local success, errorMsg = pcall(function() return file:write(frameCount .. "\n") end)
			if not success then
				emu.print_info("Error writing to pipe: " .. errorMsg)
				break
			end
			frameCount = frameCount + 1

			winapi.sleep(50)
		end
		-- local readThread = file:read_asy           nc(function(line)
		-- 	local success, errorMsg = pcall(function() return file:write(frameCount .. "\n") end)
		-- 	if not success then
		-- 		emu.print_info("Error writing to pipe: " .. errorMsg)
		-- 	end
		-- 	frameCount = frameCount + 1
		-- end)
	end
end)

function touhousplits.startplugin()

	emu.register_start(function()
		emu.print_info("Lua version")
		emu.print_info(_VERSION)

		frameCount = 0
		emu.print_info(emu.romname())
	end)

	emu.register_frame(function()
		-- if supportedRoms[emu.romname()] then
		-- 	frameCount = frameCount + 1
		-- end
	end)

	emu.register_stop(function()
		
	end)
end

return exports
