use rltk::rex::XpFile;

rltk::embedded_resource!(TREES, "../resources/trees.xp");
rltk::embedded_resource!(NOONBREEZE, "../resources/noonbreeze.xp");
pub struct RexAssets {
    pub menu : XpFile
}

impl RexAssets {
    #[allow(clippy::new_without_default)]
    pub fn new() -> RexAssets {
        rltk::link_resource!(TREES, "../resources/trees.xp");
        rltk::link_resource!(NOONBREEZE, "../resources/noonbreeze.xp");

        RexAssets{
            menu : XpFile::from_resource("../resources/trees.xp").unwrap()
        }
    }
}
