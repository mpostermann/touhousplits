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
local romName = ""

local pipe
local wrap, yield, resume = coroutine.wrap, coroutine.yield, coroutine.resume

local supportedRoms = {	["donpachij"] = true }

-- function fwrap (f,co)
--     local obj = {}
--     local started
--     function obj:read ()
--         if not started then
--             f:read_async(co)
--             started = true
--         end
--         return yield()
--     end
--     function obj:write (s)
--         return f:write(s)
--     end
--     return obj
-- end

-- function winapi.make_pipe_server_async(fun)
--     winapi.make_pipe_server(function(f)
--         local co = coroutine.wrap(fun)
--         co(fwrap(f,co))
--     end)
-- end

function touhousplits.set_folder(path)
	touhousplits.path = path
end

function touhousplits.startplugin()

	files,err = winapi.files ('*.txt',false)
	if not files then 
		return print(err) 
	end

	for f in files do
		emu.print_info(f)
	end

	emu.register_start(function()
		emu.print_info("Lua version")
		emu.print_info(_VERSION)

		frameCount = 0
		emu.print_info(emu.romname())
	end)

	emu.register_frame(function()
		if supportedRoms[emu.romname()] then
			frameCount = frameCount + 1
		end
	end)

	emu.register_stop(function()
		
	end)

	pipe = winapi.make_pipe_server(function(file)
		print("Client connected")
		while true do
			local line = file:read("l")
			print(line)
			if line == 'ping' then 
				file:write(frameCount)
				print("Frame: " .. frameCount)
			elseif line == 'close' then
				break
			end
		end

		file:close()
	end)
end

return exports
