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

local currentFrame = 1
local framesBetweenCount = 3

local score = 1

local supportedRoms = {	["donpachij"] = true }

-- local function handle_pipe(file)
-- 	emu.print_info("TouhouSplits client connected")
-- 	local readThread = file:read_async(function(line)
-- 		emu.print_info(line)
-- 		file:write(frameCount .. "\n")
-- 	end)
-- end

local pipe = nil

function touhousplits.set_folder(path)
	touhousplits.path = path
end

function touhousplits.startplugin()
	emu.register_start(function()
		emu.print_info("Lua version")
		emu.print_info(_VERSION)

		score = 0
		-- if supportedRoms[emu.romname()] then
		-- 	pipe = winapi.make_pipe_server(handle_pipe)
		-- 	pipe:set_priority(2)
		-- 	emu.print_info("Started pipe for ROM " .. emu.romname())
		-- 	emu.print_info("Thread priority: " .. pipe:get_priority())
		-- end
	end)

	emu.register_frame(function()
		if supportedRoms[emu.romname()] then
			currentFrame = currentFrame + 1
			score = score + 1

			if currentFrame >= framesBetweenCount then
				if pipe ~= nil then
					local success, error = pcall(function() pipe:write(score .. "\n") end)
					if not success then
						emu.print_info("Error writing to pipe: " .. error)
						pcall(function() pipe:close() end)
						pipe = nil
						emu.print_info("Closed pipe")
					end
				else
					local success, result = pcall(winapi.open_pipe)
					if success then
						pipe = result
						emu.print_info("Connected to TouhouSplits pipe server")
					else
						emu.print_info("Error opening pipe: " .. result)
						pipe = nil
					end
				end
				currentFrame = 1
			end
		end
	end)

	emu.register_stop(function()
		if pipe ~= nil then
			local success, error = pcall(function() pipe:close() end)
			if success then
				emu.print_info("Closed TouhouSplits pipe")
			else
				emu.print_info(error)
			end

			pipe = nil
		end
	end)

end



return exports
