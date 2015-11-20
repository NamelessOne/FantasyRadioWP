using System;
using System.IO;

namespace FantasyRadio
{
    class BassManager
    {
        public int Chan { get; set; }
        public Bass.BASS.DOWNLOADPROC StatusProc { get; } = new MyStatusProc();

        private class MyStatusProc : Bass.BASS.DOWNLOADPROC
        {
            /**
             * Тут можно получить байты потока. Используется для записи.
             * @param buffer Данные потока
             * @param length Длина куска данных потока
             * @param user BASS.dll магия. ХЗ что это
             */
            public void DOWNLOADPROC(MemoryStream buffer, int length, object user)
            {
                /*if (PlayerState.getInstance().isRecActive())
                {
                    byte[] ba = new byte[length];
                    FileOutputStream fos = null;
                    try
                    {
                        buffer.get(ba);
                        //1111
                        fos = new FileOutputStream(PlayerState.getInstance().getF().toString(), true);
                        fos.write(ba);
                        PlayerState.getInstance().setRecArtist("");
                        PlayerState.getInstance().setRecTime("");
                        PlayerState.getInstance().setRecTitle(PlayerState.getInstance().getCurrentSong());
                        PlayerState.getInstance().setRecURL("");
                    }
                    catch (Exception e1)
                    {
                        e1.printStackTrace();
                    }
                    try
                    {
                        if (fos != null)
                        {
                            fos.flush();
                            fos.close();
                        }
                    }
                    catch (Exception e1)
                    {
                        e1.printStackTrace();
                    }
                }*/
            }
        }
    }
}
